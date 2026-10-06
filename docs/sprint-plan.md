# TempTrack: PoC Sprint Plan

Mode: **PoC** (1 sprint = 5 working days). Status: **proposed** for the merged PoC kickoff gate (Gates 1-3).
Inputs: `docs/requirements.md`, `docs/architecture.md`, ADR-001 to ADR-006. Board: `docs/backlog.md`.
Day 1 starts the working day after the kickoff gate is approved. All tasks stay in `backlog` until then;
gate approval releases them to `ready` in rank order.

## 1. Sprint goal

Run the demo scenario (requirements section 7) end to end in a browser on the local `docker compose` stack, on
synthetic data: **request -> live cost -> approval chain -> reject/resubmit -> audit -> outbox -> Excel export**,
with server-side visibility rules, CI green (build, lint, smoke tests, SBOM and licence check) and the Playwright
demo smoke passing with 0 serious/critical axe findings on the four NFR pages.

Not in this sprint: any hosted deployment (Dev/Staging/Portainer), Entra ID, email, admin UI, full test pyramid
(TT-7 stays empty; requirements section 2.3).

## 2. Lanes and capacity

| Lane | Agent | Tasks | Estimates | Human-equivalent |
| --- | --- | --- | --- | --- |
| Backend | backend-engineer (Sonnet) | 17 | 10 M + 7 S | ~9.3 days |
| Frontend | frontend-engineer (Sonnet) | 13 | 5 M + 8 S | ~5.8 days |
| DevOps | devops-engineer (Sonnet) | 3 | 3 M | ~2.3 days |
| **Total** | | **33** (TT-24 to TT-56) | **18 M + 15 S** | **~17.3 days** |

Sizing (estimate-task skill): S < 2 h and < 150 changed lines; M half to one day and < 400 lines; no L.
Human-equivalent uses S = 0.25 d, M = 0.75 d. The week has ~10 lane-days, so the plan assumes agent throughput of
about 3-4 merged PRs per lane per day (build, Opus review, CI). The backend lane is the tightest (section 7).

Walking skeleton (CLAUDE.md first-sprint rule): repo + CI/SBOM (TT-24, TT-25, TT-26), local stack (TT-28) and one
end-to-end path browser -> API -> DB through mock sign-in (TT-27, TT-29, TT-31). The "dev environment" for the PoC
is the local compose stack; hosted deploy is deferred by the Tech Lead.

## 3. Day-by-day plan

Order inside a cell is merge order. A frontend task starts against the OpenAPI types as soon as its backend
dependency merges (backend PRs regenerate `docs/api/openapi.json`).

| Day | Backend | Frontend | DevOps | Milestone (end of day) |
| --- | --- | --- | --- | --- |
| 1 | TT-24 API scaffold; TT-27 reference data + seed | TT-25 SPA scaffold | TT-26 CI build/lint/test/SBOM (after TT-24, TT-25) | Both apps build in CI; SBOM covers NuGet + npm |
| 2 | TT-29 mock auth, /me, reference data; TT-30 audit store; TT-32 create draft | TT-31 sign-in + switch user; start TT-34 form | TT-28 compose stack + Playwright harness | **M1 walking skeleton**: sign in on compose stack, header shows user; e2e-smoke job green |
| 3 | TT-33 cost + preview; TT-36 edit draft; TT-35 visibility, list, detail; TT-39 audit history API | TT-34 create form; TT-37 live cost; TT-38 My requests | CI upkeep, review support (spare capacity, see Q2) | **M2**: Manager creates a draft with live cost and finds it in the list; Ward B cannot see it (API) |
| 4 | TT-40 submit + routing; TT-42 queue; TT-43 approve; TT-45 reject | TT-41 detail page; TT-44 edit + submit; TT-46 queue page; TT-50 audit history panel | spare capacity | **M3**: demo steps 1-2 and 5 in the UI (submit, two approvals, Approved; visibility) |
| 5 | TT-47 resubmit; TT-51 export; TT-52 outbox; TT-55 demo volume | TT-48 approve/reject actions; TT-49 resubmit; TT-53 export page; TT-54 outbox page | TT-56 demo scenario e2e + axe; demo dry run | **M4**: demo steps 1-7 green in CI; dry run done |

Day 5 is the overloaded day in both lanes; the slip order in section 8 applies from mid-day 4.

## 4. Task table

Ordered by rank (dependency order). Labels: `backend` / `frontend` / `devops`. All in state `backlog`.

| Rank | Key | Title | Parent | Label | Est | Depends on |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | TT-24 | Scaffold .NET API with health, logging and OpenAPI | TT-8 | backend | M | - |
| 2 | TT-25 | Scaffold React SPA with app shell and API client | TT-9 | frontend | M | - |
| 3 | TT-26 | Set up CI build, lint, test and SBOM for API and web | TT-1 | devops | M | TT-24, TT-25 |
| 4 | TT-27 | Add reference data model and idempotent seed loader | TT-8 | backend | M | TT-24 |
| 5 | TT-28 | Containerise API and web, compose stack, Playwright smoke harness | TT-1 | devops | M | TT-24, TT-25, TT-26 |
| 6 | TT-29 | Add mock dev-user auth, /api/me and reference-data API | TT-9 | backend | M | TT-27 |
| 7 | TT-30 | Add append-only audit event store and writer | TT-20 | backend | S | TT-27 |
| 8 | TT-31 | Build mock sign-in, user switch and header identity | TT-9 | frontend | M | TT-25, TT-29 |
| 9 | TT-32 | Create draft staffing request API | TT-11 | backend | M | TT-29, TT-30 |
| 10 | TT-33 | Add cost calculator and cost-preview endpoint | TT-12 | backend | M | TT-29 |
| 11 | TT-34 | Build new request form to create a draft | TT-11 | frontend | M | TT-31, TT-32 |
| 12 | TT-35 | Add visibility policy and request list and detail API | TT-10 | backend | M | TT-32, TT-33 |
| 13 | TT-36 | Add edit draft API with change audit | TT-13 | backend | S | TT-32 |
| 14 | TT-37 | Show live cost on the request form | TT-12 | frontend | S | TT-34, TT-33 |
| 15 | TT-38 | Build My requests list with status filter | TT-14 | frontend | S | TT-31, TT-35 |
| 16 | TT-39 | Add request audit history API | TT-21 | backend | S | TT-30, TT-35 |
| 17 | TT-40 | Submit request with routing and approval steps | TT-15 | backend | M | TT-35, TT-36 |
| 18 | TT-41 | Build request detail page with cost and approval chain | TT-14 | frontend | M | TT-38 |
| 19 | TT-42 | Add pending approvals queue API | TT-16 | backend | S | TT-40 |
| 20 | TT-43 | Add approve step with concurrency guard | TT-17 | backend | M | TT-40 |
| 21 | TT-44 | Edit and submit a draft from the detail page | TT-13 | frontend | S | TT-41, TT-37, TT-40 |
| 22 | TT-45 | Add reject step with reason | TT-18 | backend | S | TT-43 |
| 23 | TT-46 | Build approvals queue page | TT-16 | frontend | S | TT-31, TT-42 |
| 24 | TT-47 | Add resubmit for rejected requests | TT-19 | backend | S | TT-45, TT-36 |
| 25 | TT-48 | Add approve and reject actions on request detail | TT-17 | frontend | M | TT-41, TT-43, TT-45 |
| 26 | TT-49 | Show rejection reason and allow resubmit | TT-19 | frontend | S | TT-44, TT-47 |
| 27 | TT-50 | Show audit history on request detail | TT-21 | frontend | S | TT-41, TT-39 |
| 28 | TT-51 | Add Excel export of approved requests | TT-23 | backend | M | TT-35, TT-43 |
| 29 | TT-52 | Write workflow notifications to outbox and list API | TT-22 | backend | M | TT-47 |
| 30 | TT-53 | Build export page with filters and download | TT-23 | frontend | S | TT-31, TT-51 |
| 31 | TT-54 | Build Admin outbox page | TT-22 | frontend | S | TT-31, TT-52 |
| 32 | TT-55 | Seed demo request volume for performance check | TT-8 | backend | S | TT-43 |
| 33 | TT-56 | Add demo scenario end-to-end smoke with axe checks | TT-3 | devops | M | TT-48, TT-49, TT-50, TT-53, TT-54 |

Story coverage: every story TT-8 to TT-23 has at least one task. Cross-cutting foundation tasks hang off epics
(TT-26, TT-28 under TT-1; TT-56 under TT-3). TT-18's UI is part of TT-48. TT-20's acceptance criteria are covered by
TT-30 plus the audit event each feature task appends (named in each task's acceptance checks).

Smoke tests (PoC: smoke only). Each task names the smoke test or Playwright step it adds; backend smoke tests use
xUnit v3 + WebApplicationFactory + Testcontainers PostgreSQL, UI steps use Playwright from TT-28. TT-56 consolidates
them into the section 7 scenario. Coverage is collected and reported, not gated (Q-A5, see Q4).

Accessibility. Every frontend task's definition of done: semantic HTML, labelled controls, full keyboard use, visible
focus, status not by colour alone. axe scans run on the four NFR pages: request form (TT-34), detail (TT-50), approvals
queue (TT-46), export (TT-53), and all four again in TT-56.

## 5. Critical path

TT-24 -> TT-27 -> TT-29 -> TT-32 -> TT-35 -> TT-40 -> TT-43 -> TT-45 -> TT-47 -> TT-49 -> TT-56
(11 tasks, 8 of them backend). The outbox tail TT-47 -> TT-52 -> TT-54 -> TT-56 is one task longer, but outbox
is the first feature to slip (section 8), so it is not allowed to drive the date.

Parallel slack: TT-33 (cost) must land before TT-35; TT-36 (edit) before TT-40. Both are S/M tasks in the same
lane and are ordered to avoid conflicts in `StaffingRequest.cs` and the requests service.

## 6. Demo script reference

The demo and the TT-56 smoke test follow `docs/requirements.md` section 7 exactly:

| Demo step | Delivered by |
| --- | --- |
| 1. Manager Ward A creates a Bank request, sees cost, submits | TT-32, TT-33, TT-34, TT-37, TT-40, TT-44 |
| 2. Head of Nursing approves, CNO approves -> Approved | TT-42, TT-43, TT-46, TT-48 |
| 3. Manager creates an Agency request; HoN rejects with reason | TT-40 (extended chain), TT-45, TT-48 |
| 4. Manager revises and resubmits; HoN, CNO, Finance approve | TT-47, TT-49, TT-43 |
| 5. Manager Ward B sees neither request | TT-35, TT-38 |
| 6. Audit history per request; Admin outbox | TT-30, TT-39, TT-50; TT-52, TT-54 |
| 7. Finance exports approved requests to Excel | TT-51, TT-53 |

## 7. Risks to the week

| # | Risk | Impact | Mitigation |
| --- | --- | --- | --- |
| P1 | Backend lane holds 17 tasks (~9.3 human-days) and 8 of 11 critical-path tasks | Days 4-5 slip, demo incomplete | Contract-first merges; allow a second backend agent on independent tasks from day 3 (Q2); slip order in section 8 |
| P2 | Frontend waits on backend contracts early, then overloads on day 5 | Idle day 1-2, crunch day 5 | Frontend starts each task from the merged OpenAPI; TT-34 starts on day 2 against TT-32's contract; day-5 tasks are S |
| P3 | Testcontainers and Docker in CI and on Windows dev machines | Smoke tests cannot run | Proven on day 1 by TT-24's health smoke test; fall back to a compose service DB in CI if needed |
| P4 | Merge conflicts on shared files (`StaffingRequest.cs`, requests service, `ci.yml`, `docs/backlog.md`) | Rework, blocked merges | Dependencies encode the order of tasks touching the same files; `backlog.md` conflicts resolved by `render` (adapter rule) |
| P5 | Review and CI throughput: 33 PRs, ~7 per day | Queue builds up behind the reviewer | Small PRs (15 are S); reviewer runs as soon as a PR opens |
| P6 | Token spend: 33 tasks vs the ~20 assumed in architecture section 13.2 | Forecast rises from ~USD 350 to ~USD 450-550 (~20-25% of the USD 2,200 cap) | Orchestrator re-forecasts at the day-3 checkpoint; pause rule at 80% of cap unchanged |
| P7 | Concurrency (TT-43) and midnight/rounding (TT-33) edge cases | Flaky or wrong smoke tests | Unit tests on the pure domain with injected `TimeProvider`; concurrency test uses two explicit transactions |
| P8 | Licence finding from ClosedXML's SixLabors.Fonts (ADR-005) | SBOM job fails on day 5 | TT-51 pins SixLabors.Fonts 1.x; TT-26 makes the licence check run from day 1 |
| P9 | Open questions (Q2, Q5, Q-A3) answered differently mid-week | Rework | Chains, threshold and rates are seed data; a change is a data edit in TT-27's seed files |
| P10 | Kickoff gate approved late | Fewer than 5 days | Plan is day-relative; slip order applies |

## 8. What slips first if behind

Checkpoint: end of day 3 (M2) and mid-day 4. If M2 is not met, the Orchestrator applies the list top-down and tells
the Tech Lead. The core loop (TT-24 to TT-50) never slips.

| Order | Slips | Why it goes first | Demo impact |
| --- | --- | --- | --- |
| 1 | TT-55 demo request volume | Only serves the performance NFR, not the demo | Perf targets checked manually or after the PoC |
| 2 | TT-54 outbox page, then TT-52 outbox | Stub feature; nothing is sent in the PoC | Demo step 6 shows audit history only; outbox explained |
| 3 | TT-53 export page | TT-51 API can be demoed via a direct download link | Demo step 7 less polished |
| 4 | TT-51 export | PoC goal 6, kept above outbox | Demo step 7 dropped |
| 5 | TT-56 scope | Steps of slipped features are marked `skip` with reason, not deleted | Smoke covers steps 1-5 |

Slipped tasks stay in `backlog` under their story for the full build; nothing moves to TT-7.
