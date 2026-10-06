#!/usr/bin/env node
// Usage: node scripts/check-licences.mjs <sbom.cdx.json>
// Fails when a component uses a blocked licence and isn't listed in .github/licence-exceptions.txt.
import { existsSync, readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const BLOCKED = /(^|[^A-Za-z])(A?GPL-3\.0|SSPL)/i;

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const sbomFile = resolve(process.cwd(), process.argv[2] ?? 'sbom.cdx.json');
const exceptionsFile = resolve(root, '.github/licence-exceptions.txt');

const exceptions = new Set(
  existsSync(exceptionsFile)
    ? readFileSync(exceptionsFile, 'utf8')
        .split(/\r?\n/)
        .map((line) => line.replace(/#.*/, '').trim())
        .filter(Boolean)
    : [],
);

const sbom = JSON.parse(readFileSync(sbomFile, 'utf8'));
const components = sbom.components ?? [];
const violations = [];

for (const c of components) {
  const ids = (c.licenses ?? []).map((l) => l.expression ?? l.license?.id ?? l.license?.name ?? '');
  const blocked = ids.filter((id) => BLOCKED.test(id));
  if (blocked.length && !exceptions.has(c.name)) {
    violations.push(`${c.name}@${c.version ?? '?'}: ${blocked.join(', ')}`);
  }
}

console.log(`checked ${components.length} components, ${exceptions.size} ADR exception(s)`);
if (violations.length) {
  console.error('blocked licences (approve by ADR and add to .github/licence-exceptions.txt):');
  for (const v of violations) console.error(`  ${v}`);
  process.exit(1);
}
console.log('ok: no blocked licences');
