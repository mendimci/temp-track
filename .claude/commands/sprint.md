---
description: Run a build sprint - pick ready work items and implement them in parallel, review, and merge
argument-hint: "[max parallel tasks, default 2]"
---
Use the orchestrator subagent. Gate 3 (or the PoC kickoff gate) must be approved.

1. Query the tracker with the `work-tracker` skill for items in state `ready`, ordered by plan priority.
2. Pick up to $ARGUMENTS independent tasks (2 if no number was given). For each, delegate to backend-engineer, frontend-engineer or devops-engineer by tag. Run independent tasks in parallel, each in its own git worktree under `.worktrees/<branch>` (git-ignored), branched from `development`.
3. When each PR opens, delegate review to code-reviewer. Loop fixes until APPROVE and CI is green.
4. Move items to `done` (markdown tracker: as the last commit on the PR branch, then wait for CI to be green on it), merge (squash) into `development`, confirm the Dev deploy succeeded. Never merge into `main`.
5. Print a sprint status: done, in review, blocked, token spend vs cap.
