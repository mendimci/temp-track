---
description: Record the Tech Lead's decision on a gate (approve / reject with notes) and move to the next phase
argument-hint: "<gate: 1|2|3|4|kickoff> <approve|reject> [notes]"
---
Arguments: $ARGUMENTS

1. Open `project/decisions/gate-<gate>.md`. If it doesn't exist, stop and say which gate pack is missing.
2. Set `status: approved` or `status: rejected`, `decided_by` (Tech Lead from project.yaml), `decided_at` (today), and the notes.
3. If approved, set every ADR in `project/decisions/` with `status: proposed` that this gate pack references to `status: accepted`.
4. Commit as `docs(gate): gate <gate> <decision>`. The gate files in `project/decisions/` are the decision log; if `tools.docs.provider` is an external tool, also post the decision there.
5. If approved: tell the orchestrator subagent to start the next phase. If rejected: have the orchestrator route the notes back to the agent that produced the work and prepare a revised gate pack.
Only a human may invoke this command.
