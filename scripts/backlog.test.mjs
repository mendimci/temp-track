import assert from 'node:assert/strict';
import { execFileSync, spawnSync } from 'node:child_process';
import { appendFileSync, existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { dirname, join } from 'node:path';
import { afterEach, beforeEach, describe, test } from 'node:test';
import { fileURLToPath } from 'node:url';

const CLI = join(dirname(fileURLToPath(import.meta.url)), 'backlog.mjs');
let root;

function run(...args) {
  const r = spawnSync(process.execPath, [CLI, ...args], { env: { ...process.env, BACKLOG_ROOT: root }, encoding: 'utf8' });
  return { code: r.status, out: r.stdout.trim(), err: r.stderr.trim() };
}

function ok(...args) {
  const r = run(...args);
  assert.equal(r.code, 0, `${args.join(' ')} failed: ${r.err}`);
  return r.out;
}

const item = (key) => readFileSync(join(root, 'docs/backlog/items', `${key}.md`), 'utf8');
const board = () => readFileSync(join(root, 'docs/backlog.md'), 'utf8');

beforeEach(() => {
  root = mkdtempSync(join(tmpdir(), 'backlog-'));
  mkdirSync(join(root, 'project'));
  writeFileSync(join(root, 'project/project.yaml'), 'tools:\n  tracker:\n    provider: markdown\n    project_key: EX\n');
});

afterEach(() => rmSync(root, { recursive: true, force: true }));

describe('create and board', () => {
  test('new assigns sequential keys and renders the board', () => {
    assert.equal(ok('new', 'epic', 'Leave requests'), 'EX-1');
    assert.equal(ok('new', 'story', 'Submit a request', '--parent', 'EX-1'), 'EX-2');
    assert.equal(ok('new', 'task', 'POST endpoint', '--parent', 'EX-2', '--labels', 'backend', '--estimate', 'M'), 'EX-3');
    assert.match(item('EX-3'), /type: task\n/);
    assert.match(item('EX-3'), /parent: EX-2\n/);
    assert.match(board(), /\| \[EX-1\]\(backlog\/items\/EX-1\.md\) \| Leave requests \| backlog \| 0\/2 \|/);
    assert.match(board(), /## Backlog \(2\)/);
    assert.equal(ok('check'), 'ok: 3 backlog items');
  });

  test('escapes pipes in titles', () => {
    ok('new', 'task', 'GET /a | b');
    assert.match(board(), /GET \/a \\\| b/);
  });

  test('empty project renders "No items yet"', () => {
    ok('render');
    assert.match(board(), /No items yet\./);
  });

  test('fails without a project key', () => {
    writeFileSync(join(root, 'project/project.yaml'), 'tools: {}\n');
    const r = run('new', 'task', 'x');
    assert.equal(r.code, 1);
    assert.match(r.err, /project_key/);
  });
});

describe('update operations', () => {
  beforeEach(() => {
    ok('new', 'story', 'Story');
    ok('new', 'task', 'Task', '--parent', 'EX-1', '--labels', 'backend');
  });

  test('move walks every state and logs each transition', () => {
    for (const s of ['ready', 'in_progress', 'in_review', 'done']) ok('move', 'EX-2', s, '--by', 'backend-engineer');
    assert.match(item('EX-2'), /status: done\n/);
    assert.match(item('EX-2'), /\(backend-engineer\): in_review -> done/);
    assert.match(board(), /## Done \(1\)/);
    assert.match(board(), /done 1\/2/);
  });

  test('set pr renders a PR link, labels and rank are typed', () => {
    ok('set', 'EX-2', 'pr', 'https://github.com/o/r/pull/14');
    ok('set', 'EX-2', 'labels', 'backend, api');
    ok('set', 'EX-2', 'rank', '5');
    assert.match(board(), /\[#14\]\(https:\/\/github\.com\/o\/r\/pull\/14\)/);
    assert.match(item('EX-2'), /labels:\n {2}- backend\n {2}- api\n/);
    assert.match(item('EX-2'), /rank: 5\n/);
  });

  test('comment appends to the log without touching the board', () => {
    const before = board();
    ok('comment', 'EX-2', 'review: APPROVE', '--by', 'code-reviewer');
    assert.match(item('EX-2'), /## Log\n[\s\S]*\(code-reviewer\): review: APPROVE/);
    assert.equal(board(), before);
  });

  test('list filters by status, type and label', () => {
    ok('move', 'EX-2', 'ready');
    assert.equal(ok('list', '--status', 'ready'), 'EX-2\tready\ttask\tbackend\tTask');
    assert.equal(ok('list', '--type', 'story'), 'EX-1\tbacklog\tstory\t\tStory');
    assert.equal(ok('list', '--label', 'frontend'), '');
  });

  test('delete refuses an item with children and removes a leaf', () => {
    const r = run('delete', 'EX-1');
    assert.equal(r.code, 1);
    assert.match(r.err, /has children \(EX-2\)/);
    ok('delete', 'EX-2');
    assert.equal(existsSync(join(root, 'docs/backlog/items/EX-2.md')), false);
    assert.equal(ok('check'), 'ok: 1 backlog items');
  });

  test('rekey moves an item to the next key and updates its children', () => {
    assert.equal(ok('rekey', 'EX-1'), 'EX-1 -> EX-3');
    assert.equal(existsSync(join(root, 'docs/backlog/items/EX-1.md')), false);
    assert.match(item('EX-3'), /re-keyed from EX-1/);
    assert.match(item('EX-2'), /parent: EX-3\n/);
    assert.equal(ok('check'), 'ok: 2 backlog items');
  });
});

describe('rejects bad input without writing anything', () => {
  beforeEach(() => {
    ok('new', 'epic', 'Epic');
    ok('new', 'story', 'Story', '--parent', 'EX-1');
  });

  const cases = [
    ['invalid state', ['move', 'EX-2', 'doing'], /state must be one of/],
    ['unknown key', ['move', 'EX-9', 'done'], /unknown item EX-9/],
    ['invalid type', ['new', 'feature', 'x'], /usage: new/],
    ['missing parent', ['new', 'task', 'x', '--parent', 'EX-9'], /unknown item EX-9/],
    ['non-numeric rank', ['set', 'EX-2', 'rank', 'high'], /rank must be a number/],
    ['self parent', ['set', 'EX-1', 'parent', 'EX-1'], /loops back to EX-1/],
    ['parent cycle', ['set', 'EX-1', 'parent', 'EX-2'], /loops back/],
    ['unknown field', ['set', 'EX-1', 'owner', 'me'], /field must be one of/],
  ];

  for (const [name, args, message] of cases) {
    test(name, () => {
      const before = [item('EX-1'), item('EX-2'), board()];
      const r = run(...args);
      assert.equal(r.code, 1);
      assert.match(r.err, message);
      assert.deepEqual([item('EX-1'), item('EX-2'), board()], before);
    });
  }
});

describe('check', () => {
  beforeEach(() => {
    ok('new', 'epic', 'Epic');
    ok('new', 'story', 'Story', '--parent', 'EX-1');
  });

  test('fails when the board was edited by hand', () => {
    appendFileSync(join(root, 'docs/backlog.md'), 'manual edit\n');
    assert.match(run('check').err, /out of date/);
  });

  test('fails on an invalid status in a ticket file', () => {
    writeFileSync(join(root, 'docs/backlog/items/EX-2.md'), item('EX-2').replace('status: backlog', 'status: blocked'));
    assert.match(run('check').err, /EX-2\.md: status must be one of/);
  });

  test('reports a parent cycle in the files instead of crashing', () => {
    writeFileSync(join(root, 'docs/backlog/items/EX-1.md'), item('EX-1').replace('status: backlog', 'status: backlog\nparent: EX-2'));
    const r = run('check');
    assert.equal(r.code, 1);
    assert.match(r.err, /loops back/);
  });

  test('fails on a file name that does not match its key', () => {
    writeFileSync(join(root, 'docs/backlog/items/EX-7.md'), item('EX-2').replace('key: EX-2', 'key: EX-8'));
    assert.match(run('check').err, /EX-7\.md: file name must be EX-8\.md/);
  });
});

test('new skips keys that already exist on origin/development', () => {
  const git = (...a) => execFileSync('git', a, { cwd: root, stdio: 'ignore' });
  git('init', '-q');
  ok('new', 'task', 'One');
  ok('new', 'task', 'Two');
  ok('new', 'task', 'Three');
  git('add', '-A');
  git('-c', 'user.name=t', '-c', 'user.email=t@t', 'commit', '-qm', 'base');
  git('update-ref', 'refs/remotes/origin/development', 'HEAD');
  rmSync(join(root, 'docs/backlog/items/EX-3.md'));
  ok('render');
  assert.equal(ok('new', 'task', 'Parallel work'), 'EX-4');
});
