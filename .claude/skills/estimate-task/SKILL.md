---
name: estimate-task
description: Size and split tasks so each fits one reviewable PR. Use when planning sprints.
---
- S: < 2h human-equivalent, < 150 lines; M: half day to a day, < 400 lines; L: split it.
- Split by layer only when layers can ship independently; prefer vertical slices.
- Each task lists: acceptance checks, dependencies (keys), risk (low/med/high).
- Record expected token cost tier: S ≈ low, M ≈ medium; re-calibrate from pilot data in `docs/metrics.md`.
