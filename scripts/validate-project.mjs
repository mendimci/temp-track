#!/usr/bin/env node
// Usage: node scripts/validate-project.mjs [file]   (default: project/project.yaml)
import { existsSync, readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import Ajv2020 from 'ajv/dist/2020.js';
import addFormats from 'ajv-formats';
import { parse } from 'yaml';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const file = resolve(root, process.argv[2] ?? 'project/project.yaml');
const schema = JSON.parse(readFileSync(resolve(root, 'project/project.schema.json'), 'utf8'));

if (!existsSync(file)) {
  console.log(`skip: ${file} not found (run /setup-project to create it)`);
  process.exit(0);
}

let data;
try {
  data = parse(readFileSync(file, 'utf8'));
} catch (err) {
  console.error(`invalid YAML in ${file}: ${err.message}`);
  process.exit(1);
}

const ajv = new Ajv2020({ allErrors: true, strict: false });
addFormats(ajv);
const validate = ajv.compile(schema);

if (validate(data)) {
  console.log(`ok: ${file}`);
  process.exit(0);
}

console.error(`invalid: ${file}`);
for (const e of validate.errors) {
  const extra = e.params?.additionalProperty ? ` '${e.params.additionalProperty}'` : '';
  console.error(`  ${e.instancePath || '/'} ${e.message}${extra}`);
}
process.exit(1);
