#!/usr/bin/env bash
# Usage: scripts/health-check.sh <base-url> [attempts]
set -euo pipefail
url="${1%/}/health"
attempts="${2:-20}"
for i in $(seq 1 "$attempts"); do
  if body=$(curl -fsS --max-time 10 "$url"); then
    echo "healthy: $url -> $body"
    exit 0
  fi
  echo "waiting for $url ($i/$attempts)"
  sleep 15
done
echo "unhealthy: $url did not respond after $attempts attempts" >&2
exit 1
