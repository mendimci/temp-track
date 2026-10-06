#!/usr/bin/env bash
# PreToolUse hook for Write/Edit: blocks content that looks like a secret.
input=$(cat)
patterns='(AKIA[0-9A-Z]{16}|-----BEGIN [A-Z ]*PRIVATE KEY-----|ghp_[A-Za-z0-9]{36}|github_pat_[A-Za-z0-9_]{40,}|sk-ant-[A-Za-z0-9_-]{20,}|xox[baprs]-[A-Za-z0-9-]{10,}|AccountKey=[A-Za-z0-9+/=]{40,})'
if echo "$input" | grep -Eq "$patterns"; then
  echo "Blocked: the content looks like a secret. Reference it by environment secret name instead (see CLAUDE.md > Engineering standards)." >&2
  exit 2
fi
exit 0
