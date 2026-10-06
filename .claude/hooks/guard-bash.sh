#!/usr/bin/env bash
# PreToolUse hook for Bash: blocks dangerous commands. Exit 2 = block (stderr is shown to Claude).
input=$(cat)
deny='(git push[^"]*(--force|-f( |"|$))|deploy-prod|--target[= ]production|rm -rf /( |"|$)|git reset --hard origin/(main|development)|DROP DATABASE)'
if echo "$input" | grep -Eiq "$deny"; then
  echo "Blocked by team policy (CLAUDE.md > Safety): force-push, production deploys and destructive commands are not allowed for agents. Ask the Orchestrator / Tech Lead." >&2
  exit 2
fi
exit 0
