---
description: Show project status - phase, gates, work items, environments, token spend
---
Read `project/project.yaml`, `project/decisions/`, and query the tracker with the `work-tracker` skill. Print:
- Current phase and gate statuses
- Work items by state (backlog / ready / in progress / in review / done) and blocked items
- Last Dev and Staging deploy (from CI)
- Token spend vs cap
- Decisions needed from the Tech Lead
Keep it under 20 lines.
