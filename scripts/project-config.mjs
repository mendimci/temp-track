#!/usr/bin/env node
// Usage: node scripts/project-config.mjs <dot.path> [file]   e.g. environments.dev.url
// Prints the value (empty string when missing) so workflows can read project.yaml.
import { existsSync, readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { parse } from 'yaml';

const [path, fileArg] = process.argv.slice(2);
if (!path) {
  console.error('usage: project-config.mjs <dot.path> [file]');
  process.exit(2);
}

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const file = resolve(root, fileArg ?? 'project/project.yaml');
if (!existsSync(file)) process.exit(0);

const value = path.split('.').reduce((node, key) => node?.[key], parse(readFileSync(file, 'utf8')));
if (value !== undefined && value !== null) {
  console.log(typeof value === 'object' ? JSON.stringify(value) : String(value));
}
