---
gate: kickoff
status: pending        # pending | approved | rejected
prepared_by: orchestrator
prepared_at: 2026-10-06
decided_by:
decided_at:
---
# Gate kickoff (PoC): Scope + Architecture + Plan

## Summary (3 lines max)
1-week PoC of TempTrack, run locally on synthetic data: request (Overtime/Bank/Agency) with auto cost, 2–3 step approval (approve/reject/resubmit), audit trail, role visibility via mock login, Excel export, notification outbox.
Stack: .NET 10 minimal APIs + EF Core, React 19 + TypeScript (Vite), PostgreSQL 17, docker compose. Entra ID SSO, email, admin UI and deployment are deferred.
7 epics, 16 stories, 33 tasks (TT-1..TT-56). Forecast token spend ~USD 450–550 (20–25% of cap).

## What was produced
- Requirements, PoC slice and deferred list: [docs/requirements.md](../../docs/requirements.md) (demo script §7, questions §9)
- Architecture: [docs/architecture.md](../../docs/architecture.md) (costs §13, risks §14, questions §15)
- ADRs (proposed): [adr-001 stack](adr-001-stack.md), [adr-002 hosting](adr-002-hosting.md), [adr-003 data store](adr-003-data-store.md), [adr-004 auth](adr-004-auth.md), [adr-005 Excel export](adr-005-excel-export.md), [adr-006 approval workflow config](adr-006-approval-workflow-config.md)
- Sprint plan: [docs/sprint-plan.md](../../docs/sprint-plan.md); critical path TT-24 → 27 → 29 → 32 → 35 → 40 → 43 → 45 → 47 → 49 → 56
- Backlog: [docs/backlog.md](../../docs/backlog.md). Epics TT-1..TT-7 (TT-7 = deferred, empty), stories TT-8..TT-23, tasks TT-24..TT-56 (backend 17, frontend 13, devops 3). All in `backlog`; approval releases them to `ready`.

**PoC slice in:** mock sign-in with role switching (Manager, Head of Nursing, CNO, Finance, Admin); create/edit/submit request; cost = headcount × hours × rate(type, band), frozen at submit; chain Head of Nursing → CNO, + Finance for Agency or ≥ threshold (placeholder GBP 1,000); approval queue, approve, reject with reason, resubmit (restarts at step 1); append-only audit history; server-side role/department visibility; outbox log; Excel export (ClosedXML) of approved requests with filters; smoke tests + one Playwright demo test.
**Deferred:** Entra ID SSO; real email + templates + retry; real-time notifications; admin UI (config via seed files); time-based escalation; withdraw/delegation; client Excel templates; type-specific forms; client UI design; deployment to Dev/Staging/Prod; full test pyramid; user guide, UAT, handover.

## Decisions needed from the Tech Lead
1. Approve PoC slice, stack and plan as above — recommendation: approve; all choices follow the PRD Q&A (.NET + React) and preferred stacks.
2. Auth (Q1 / Q-A1): mock login only, or real Entra ID in the PoC (~0.5–1 day on the company tenant)? — recommendation: mock; optional Entra spike on day 5 only if the core flow is done.
3. Approval chain (Q2/Q3): Head of Nursing → CNO, + Finance step (labelled "Agency approval") for Agency and high-cost — recommendation: accept as seed-data default until the client sends the final flow.
4. Cost formula and threshold (Q4/Q5/Q-A3): flat headcount × hours × rate(type, band), GBP, threshold GBP 1,000 per request — recommendation: accept as placeholders.
5. Database (Q-A4): PostgreSQL vs SQL Server — recommendation: PostgreSQL; switching later is ~1–2 days.
6. Coverage gate in PoC (Q-A5 / plan Q4): recommendation: report-only in PoC, enforced in full mode.
7. Capacity (plan Q1/Q2): plan is ~17 human-days vs ~10 lane-days; allow a second backend agent in its own worktree from day 3 — recommendation: yes. If still behind, drop order: perf seed → outbox → export page → export.
8. Demo timing (plan Q5): dry run end of day 5, client demo next morning — recommendation: yes.

## Standards check
- [x] Industry standards: OWASP review per PR planned; append-only audit (DB trigger); WCAG 2.1 AA basics + axe in Playwright; synthetic data only; SBOM + licence check in CI (TT-26); ClosedXML MIT with SixLabors.Fonts pinned to 1.x (Apache-2.0)
- [x] Code-invention internal standards: preferred stack, ADRs for key choices, markdown tracker, trunk-based PRs < 400 lines, smoke tests per PoC mode
- [ ] Not yet verifiable: CI, coverage and SBOM run only once code exists

## Risks and open questions
- Schedule squeeze (backend lane overloaded, day 5 heavy) — mitigated by decision 7.
- Client inputs missing: final approval flow, pay rates, Excel template, presentation PDF and `TempTrack-Demo.html` (referenced in PRD, not in repo).
- Mock login must never reach a deployed environment without a network guard or Entra (app refuses to start in Production in mock mode).
- Data residency: client is a UK hospital (UK GDPR); team default is EU. Decide at hosting time (Azure UK South vs company Portainer).
- Full open-question list: requirements §9 (20 items, each with a default), architecture §15 (5 items). Defaults are assumed unless the Tech Lead overrides them.

## Cost
- Token spend so far: ~USD 5 of USD 2,200 (< 1%); estimate from subagent token counts (~290k tokens) plus orchestration, not a `/cost` reading.
- Forecast PoC total: ~USD 450–550 (20–25% of cap); pause point USD 1,760 (80%).
- Estimated hosting cost (USD/month, deferred; PoC week = 0): company Portainer dev 10–20, staging 10–20, prod 40–80 (total 60–120); Azure UK South alternative total 170–230.

## Tech Lead notes
