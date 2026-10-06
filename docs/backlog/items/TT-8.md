---
key: TT-8
type: story
title: Seed synthetic reference data
status: backlog
parent: TT-1
labels:
  - backend
---
As an Admin, I want departments, users, roles, pay rates, approval chains and the high-cost threshold loaded from seed files, so that the demo runs without an admin UI or client data.

## Acceptance criteria
- Given an empty database, when the app starts with seeding enabled, then departments, users with roles and department scope, pay rates per staff type and band, approval chains and the threshold are loaded.
- Given seeding has already run, when it runs again, then no duplicates are created (idempotent).
- Given the seed files, when reviewed, then every person name and email is fictitious and marked synthetic.

## Notes
Business rules: docs/requirements.md sections 4-5.

## Definition of done
Smoke test covers it, reviewed, docs updated if behaviour changed, WCAG 2.1 AA for UI, synthetic data only.

## Log
- 2026-10-06 (product-analyst): created
