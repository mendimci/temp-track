# GitHub Issues adapter (`github` MCP server or `gh` CLI)
- Types become labels: `type:epic`, `type:story`, `type:task`, `type:bug`; epics use sub-issues.
- States: `backlog`/`ready` = open + label `state:<name>`; `in_progress`/`in_review` = open + label; `done` = closed.
- query → `gh issue list --label "state:ready" --json number,title,labels`.
- link → reference `#<number>` in the PR body (`Closes #<number>`).
