---
name: code-host
description: Tool-agnostic branch, pull request and review operations for GitHub, Azure Repos or GitLab. Use whenever an agent opens, updates, reviews or merges a PR.
---
# Code host (neutral interface)
Read `tools.code_host` from `project/project.yaml`.

| Operation | GitHub | Azure Repos |
| --- | --- | --- |
| open PR | `gh pr create --title "<KEY>: <summary>" --body-file <file> --base development` | `az repos pr create --title ... --description ... --target-branch development` |
| review comment | `gh pr review <n> --comment -b "<text>"` / `--request-changes` / `--approve` | `az repos pr set-vote --vote approve/reject` + thread comment |
| status checks | `gh pr checks <n>` | `az repos pr policy list --id <n>` |
| merge | `gh pr merge <n> --squash --delete-branch` | `az repos pr update --id <n> --status completed --squash true` |

PR body template:
```
## What
## Why (link to work item)
## How tested
## Screenshots (UI)
## Checklist
- [ ] Tests added  - [ ] Docs updated  - [ ] No secrets  - [ ] SBOM/licence check green
```
Never merge with failing checks or without an APPROVE from the code-reviewer.
Task PRs target `development`. Release PRs `development` → `main` are opened by the Orchestrator and merged only by the Tech Lead.
