---
key: TT-9
type: story
title: Switch demo user and role via mock sign-in
status: backlog
parent: TT-1
labels:
  - backend
  - frontend
---
As a demo presenter, I want to pick a seeded user from a sign-in list and switch user at any time, so that I can show each role's view without SSO.

## Acceptance criteria
- Given the mock-auth dev flag is on, when I open the app, then I see a list of seeded users with role and department and can sign in as one.
- Given I am signed in, when I choose "switch user", then I return to the list and the previous identity no longer applies to API calls.
- Given the mock-auth flag is off, when the app starts, then it refuses to start with a clear error.
- Given I am signed in, when I view any page, then my name and role are shown in the header.

## Notes
Business rules: docs/requirements.md sections 4-5.

## Definition of done
Smoke test covers it, reviewed, docs updated if behaviour changed, WCAG 2.1 AA for UI, synthetic data only.

## Log
- 2026-10-06 (product-analyst): created
