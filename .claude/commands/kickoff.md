---
description: Start delivery from the PRD - read docs/prd.md, run Discovery and prepare Gate 1 (or the merged PoC kickoff gate)
---
Use the orchestrator subagent.

1. Check `project/project.yaml` exists; if not, stop and tell the user to run /setup-project.
2. Read the PRD from `project.prd` (default `docs/prd.md`). If the file is missing or empty, stop and ask the Tech Lead to add it using `templates/prd-template.md`.
   Only if `project.prd` is a URL (external docs integration confirmed by the Tech Lead): fetch it through the docs MCP server and save a snapshot to `docs/prd.md` with the fetch date.
3. Delegate Discovery to the product-analyst subagent.
4. If `mode: poc`: also delegate Architecture (solution-architect) and Planning (planner) in sequence, then prepare ONE merged gate pack `project/decisions/gate-kickoff.md`.
   If `mode: full`: prepare `project/decisions/gate-1.md`.
5. Stop and present the gate pack to the Tech Lead with the questions that need answers.
