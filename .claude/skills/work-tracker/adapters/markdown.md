# Markdown adapter (default: tickets as files in the repo)

Items live in `docs/backlog/items/<KEY>.md` (YAML front matter + markdown body). The board `docs/backlog.md` is generated from them.
Keys are `<tools.tracker.project_key>-<n>`, e.g. `LEAVE-12`. Canonical types and states are used as is; no `status_map` or `type_map`.

Always use the CLI (it validates and re-renders the board); edit an item file directly only to write its description:

| Operation | Command |
| --- | --- |
| create | `node scripts/backlog.mjs new <type> "<title>" [--parent KEY] [--labels backend] [--estimate M] [--rank N] --by <agent>` → prints the key |
| update | `node scripts/backlog.mjs set <KEY> <title\|parent\|labels\|estimate\|rank> <value>`; description: edit the item file body |
| transition | `node scripts/backlog.mjs move <KEY> <state> --by <agent>` |
| link | `node scripts/backlog.mjs set <KEY> pr <PR URL>` |
| query | `node scripts/backlog.mjs list --status ready [--type task] [--label backend]` (tab-separated: key, state, type, labels, title) |
| comment | `node scripts/backlog.mjs comment <KEY> "<text>" --by <agent>` |
| delete | `node scripts/backlog.mjs delete <KEY>` (refuses while the item has children) |

Run `npm ci --prefix scripts` once per clone or worktree before the first command.

## Rules
- Backlog changes are committed like code: on the task's branch, inside its PR. Discovery and Planning commit theirs on their own docs branch.
- Task flow on the task branch: `in_progress` when work starts, `in_review` + `pr` link when the PR opens.
  After APPROVE: commit `done` as the last change, wait for CI to be green on that commit, then merge.
- Create items where the work happens: Discovery and Planning on their docs branch, bugs on the QA branch. `new` skips keys already on `origin/development` (run `git fetch` first).
- Never edit `docs/backlog.md` by hand. CI fails if it doesn't match the item files (`node scripts/backlog.mjs check`).
- If a PR conflicts only on `docs/backlog.md`: merge `development` into the branch, run `node scripts/backlog.mjs render`, commit, push. Item files don't conflict, because each PR touches its own items.
- If merging `development` shows an add/add conflict on an item file, two branches created the same key: `git merge --abort`, `git fetch`, `node scripts/backlog.mjs rekey <KEY>` for this branch's item (it moves to a key that is free on both), update references to the old key in this PR, commit, merge again.
- Bugs: `new bug` with `--parent <story key>` and the bug template from the `write-story` skill in the body.
