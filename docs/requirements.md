# TempTrack: Requirements (PoC)

Source: `docs/prd.md`. Mode: **PoC** (1 week, `project/project.yaml`). Tracker: markdown, key `TT`.
Status: draft for the PoC kickoff gate. Defaults marked **(assumption, Qn)** need confirmation; see section 9.

## 1. Summary

TempTrack replaces email and verbal approval of temporary staffing (Overtime, Bank, Agency) at
St John & St Elizabeth Hospital. Managers raise a request, see its cost before it is incurred, and
the request moves through a multi-step approval chain with a full audit trail. Approved requests are
exported to Excel for existing finance reporting.

The PoC proves the core loop end to end on synthetic data: **request → cost → approval chain → audit → export**.

## 2. Goals and scope

### 2.1 PoC goals
1. A Manager can raise an Overtime, Bank or Agency request and see its calculated cost before submitting.
2. The request is routed through a 2–3 step approval chain defined in seed data, with an extra step for Agency or high-cost requests.
3. Approvers can approve or reject; a rejected request can be revised and resubmitted.
4. Every action is visible in a per-request audit history.
5. Users only see the requests their role and department allow.
6. Approved requests can be exported to Excel with basic filters.

### 2.2 PoC slice (in)
A runnable web app (local / developer machine) with a mock sign-in that lets the demo user switch between seeded
users (Manager, Head of Nursing, CNO, Finance, Admin). Reference data (departments, users, pay rates, approval
chains, high-cost threshold) is synthetic and loaded from seed files. A Manager creates, edits and submits a request
of type Overtime, Bank or Agency; cost is calculated from seeded pay rates. Submission routes the request through the
seeded approval chain (Head of Nursing → CNO; Agency or high-cost adds a Finance step). Approvers see a queue, approve
or reject with a reason; rejected requests can be revised and resubmitted. Every action writes an append-only audit
event shown on the request. Workflow notifications are written to an outbox (no email sent). Approved requests can be
exported to `.xlsx` filtered by date range, department and staff type. Smoke tests only.

### 2.3 Deferred (out of the PoC, tracked under TT-7)
| Item | PRD ref | Note |
| --- | --- | --- |
| Azure AD / Entra ID SSO, session management | 3.6 | Mock auth in PoC (Q1) |
| Real email delivery, templates, retry | 3.3 | Outbox stub in PoC (Q14) |
| Admin UI for pay rates, thresholds, chains, departments, users, with change tracking | 3.7 | Seed files in PoC |
| Time-based escalation and reminders | 3.2, 3.3 | Q13 |
| Real-time (in-app push) notifications | 5 (team) | Q14 |
| Withdraw / cancel a submitted request; delegation (e.g. Deputy CNO) | 3.2 | Q9, Q12 |
| Client-specific Excel templates, multiple export scenarios (monthly, departmental) | 3.5 | Q15 |
| Configurable request forms (per type) | 3.1 | Fixed form in PoC |
| Deployment to Dev / Staging / Production, environment setup | 3.8 | Tech Lead decision |
| Full test pyramid (integration, E2E, contract), performance testing | 8 | PoC: smoke tests |
| User guide, handover, UAT | 3.8, 8 | Release phase |
| Applying the client UI/UX design | 3.1 | Q7 |

### 2.4 Out of scope (PRD section 7)
Integrations beyond Azure AD and email, data migration, infrastructure provisioning and running costs, changes to
hospital finance/reporting systems, security audits and penetration testing, post-handover support.

## 3. Personas and roles

| Role | Who (PRD) | In the PoC can |
| --- | --- | --- |
| **Manager** (requester) | Ward / department manager | Create, edit, submit, resubmit requests for own department; see own department's requests |
| **Head of Nursing** (approver step 1) | Q&A: "Head of Nursing" | Approve/reject step 1 for departments in scope; see those departments' requests |
| **CNO** (approver step 2) | Q&A: "CNO (Deputy CNO)" | Approve/reject step 2; see all requests |
| **Finance** (approver step 3) | PRD 3.2 / 3.6 | Approve/reject the Agency / high-cost step; see all requests; export |
| **Admin** | PRD 3.7 | See all requests and the notification outbox; export. Config is seed-only in PoC |

One seeded user may hold one role. Role-to-department scope comes from seed data **(assumption, Q9, Q10)**.

## 4. Business rules

### 4.1 Request
- Types: **Overtime**, **Bank**, **Agency**.
- Fields **(assumption, Q7, Q8)**: department, staff type, band/grade, shift date, start time, end time, unpaid break
  (minutes), headcount (1–20), reason (seeded list: sickness, vacancy, increased acuity, annual leave, other),
  free-text notes (max 1,000 chars). Required: all except notes.
- One request = one shift on one date for N staff of the same type and band **(assumption, Q8)**.
- An end time earlier than the start time means the shift ends the next day.
- Shift date must be today or later at submission.

### 4.2 Cost calculation **(assumption, Q5, Q6)**
- `paid hours = (end − start) − unpaid break`, must be > 0.
- `cost = headcount × paid hours × hourly rate(staff type, band)`.
- Rates come from the seeded pay-rate table; values are synthetic placeholders. Currency shown as GBP.
- Round to 2 decimals (half-up) at the final total only.
- Cost is recalculated live while editing and shown before submit.
- On submit, the rate and cost are **snapshotted** on the request, so later rate changes don't alter it.
- No enhancements (nights, weekends, bank holidays), overtime multipliers or agency fees/VAT in the PoC.

### 4.3 Workflow states
| State | Meaning | Who can edit |
| --- | --- | --- |
| `Draft` | Saved, not submitted | Requester |
| `Pending` (step k of n) | Waiting for the role at step k | Nobody |
| `Approved` | All steps approved | Nobody (final) |
| `Rejected` | An approver rejected it with a reason | Requester (revise and resubmit) |

Transitions: Draft → Pending(1) on submit; Pending(k) → Pending(k+1) on approve; Pending(n) → Approved;
Pending(k) → Rejected on reject; Rejected → Pending(1) on resubmit **(assumption, Q11)**.

### 4.4 Approval routing **(assumption, Q2, Q3, Q4)**
Chains are defined in seed data, not in code:

| Condition | Chain |
| --- | --- |
| Overtime or Bank, cost < threshold | Head of Nursing → CNO |
| Agency (any cost) | Head of Nursing → CNO → Finance ("agency approval") |
| Any type, cost ≥ high-cost threshold | Head of Nursing → CNO → Finance |

- High-cost threshold: seeded placeholder **GBP 1,000** per request total.
- The chain is resolved at submit (and at resubmit) from the snapshotted cost and type.
- A user cannot approve a request they raised; an approver only acts on a step for their role and department scope.
- Approve takes an optional comment; reject requires a reason (min 5 chars).

### 4.5 Audit
Append-only events: created, updated (changed fields, old → new), submitted, step approved, rejected, resubmitted,
exported. Each event stores UTC timestamp, actor id, actor role, step (if any) and comment. No update or delete path.

### 4.6 Notifications (PoC stub)
Outbox entries are written on: submitted / resubmitted (to step-1 approvers), step approved (to next-step approvers),
final approval (to requester), rejected (to requester). Each entry: event, recipients, subject, body, created at.
Nothing is sent.

## 5. Non-functional requirements

| Area | Requirement | Target (PoC) |
| --- | --- | --- |
| Accessibility | WCAG 2.1 AA on all PoC pages: semantic HTML, labelled form controls, full keyboard use, visible focus, contrast ≥ 4.5:1 | 0 serious/critical axe violations on request form, request detail, approvals queue, export page |
| Security | Authorisation enforced server-side on every API call (not only hidden in UI) | Smoke test: Manager of dept A gets 403/404 for a dept B request |
| Security | Mock auth only runs when an explicit dev flag is set; it must fail to start otherwise | App refuses to start with mock auth and the flag unset |
| Security | Input validated at the API boundary; OWASP Top 10 review per PR; no secrets in code | Per CLAUDE.md |
| Data / GDPR | Synthetic data only (`client_data_allowed: false`); no real staff names | Seed files reviewed in PR |
| Data / GDPR | PII inventory (user names, emails, request notes) recorded in `docs/architecture.md` | At Gate 2 |
| Data | Store times in UTC, display Europe/London **(assumption)** | Smoke test on a shift crossing midnight |
| Data | Money stored as fixed-point decimal, not float | Cost unit test |
| Audit | Audit events are append-only | No update/delete endpoint exists |
| Performance | With seed data (~500 requests), list and detail API responses | p95 < 1 s locally |
| Performance | Excel export of 1,000 approved requests | < 5 s |
| Logging | Structured logs to stdout with correlation ID, no PII | Per CLAUDE.md |
| Compatibility | Latest Microsoft Edge and Chrome, desktop ≥ 1280 px; usable at 360 px width | Manual check |
| Testing | Smoke tests covering the demo scenario (section 7) | Green in CI |

## 6. Epics and stories

States: all items are `backlog`; the Planner moves them to `ready` after the kickoff gate.
Full acceptance criteria are in each item file under `docs/backlog/items/`.

### TT-1 Access, roles and reference data (mock auth)
- **TT-8** Seed synthetic reference data
- **TT-9** Switch demo user and role via mock sign-in
- **TT-10** Restrict request visibility by role and department

### TT-2 Request management with cost calculation
- **TT-11** Create a draft staffing request
- **TT-12** Calculate request cost from seeded pay rates
- **TT-13** Edit and submit a draft request
- **TT-14** Track my requests and their status

### TT-3 Approval workflow
- **TT-15** Route submitted request through approval chain
- **TT-16** View my pending approvals queue
- **TT-17** Approve a request step
- **TT-18** Reject a request with a reason
- **TT-19** Revise and resubmit a rejected request

### TT-4 Audit trail
- **TT-20** Record audit events for every request action
- **TT-21** View request audit history

### TT-5 Notifications (outbox stub)
- **TT-22** Write workflow notifications to an outbox log

### TT-6 Reporting and Excel export
- **TT-23** Export approved requests to Excel with filters

### TT-7 Deferred beyond PoC (label `deferred`, no stories)
See section 2.3.

### 6.1 Stories with acceptance criteria

**TT-8 Seed synthetic reference data**
As an Admin, I want departments, users, roles, pay rates, approval chains and the high-cost threshold loaded from seed files, so that the demo runs without an admin UI or client data.
- Given an empty database, when the app starts with seeding enabled, then departments, users with roles and department scope, pay rates per staff type and band, approval chains and the threshold are loaded.
- Given seeding has already run, when it runs again, then no duplicates are created (idempotent).
- Given the seed files, when reviewed, then every person name and email is fictitious and marked synthetic.

**TT-9 Switch demo user and role via mock sign-in**
As a demo presenter, I want to pick a seeded user from a sign-in list and switch user at any time, so that I can show each role's view without SSO.
- Given the mock-auth dev flag is on, when I open the app, then I see a list of seeded users with role and department and can sign in as one.
- Given I am signed in, when I choose "switch user", then I return to the list and the previous identity no longer applies to API calls.
- Given the mock-auth flag is off, when the app starts, then it refuses to start with a clear error.
- Given I am signed in, when I view any page, then my name and role are shown in the header.

**TT-10 Restrict request visibility by role and department**
As a Manager, I want to see only my department's requests, so that data is visible only to the people who need it.
- Given I am a Manager of department A, when I list requests, then I see only department A requests.
- Given I am a Manager of department A, when I request a department B request by id via the API, then I get not found.
- Given I am a Head of Nursing, when I list requests, then I see requests of departments in my scope.
- Given I am CNO, Finance or Admin, when I list requests, then I see all requests.

**TT-11 Create a draft staffing request**
As a Manager, I want to create an Overtime, Bank or Agency request with the required details, so that the need is recorded in one place.
- Given I am a Manager, when I open "New request", then I can choose type Overtime, Bank or Agency and fill department (limited to mine), band, shift date, start, end, unpaid break, headcount, reason and notes.
- Given a required field is empty or invalid (headcount outside 1–20, paid hours ≤ 0, date in the past), when I save, then the form shows an error next to that field and nothing is saved.
- Given valid input, when I save, then the request is stored as `Draft` with a reference number and I land on its detail page.
- Given I use only the keyboard, when I complete the form, then every field and button is reachable and labelled.

**TT-12 Calculate request cost from seeded pay rates**
As a Manager, I want to see the request's cost before I submit, so that spend is visible before it is incurred.
- Given type Bank, band 5, 07:00–19:30, 30 min break, headcount 2 and a seeded rate R, when I fill the form, then the cost shown is 2 × 12 × R, rounded to 2 decimals.
- Given a night shift 20:00–08:00, when the cost is calculated, then the shift is treated as ending the next day.
- Given no rate is seeded for the chosen type and band, when I try to submit, then I see "No pay rate configured" and cannot submit.
- Given a request is submitted, when the seeded rate later changes, then the request keeps its snapshotted rate and cost.

**TT-13 Edit and submit a draft request**
As a Manager, I want to edit my draft and submit it, so that it enters approval when it is ready.
- Given my request is `Draft`, when I edit and save it, then the changes and the recalculated cost are stored.
- Given my request is `Draft` and valid, when I submit, then it becomes `Pending` at step 1 of its chain.
- Given a request is `Pending` or `Approved`, when I try to edit it, then editing is not offered and the API rejects it.
- Given I am not the requester, when I try to edit or submit someone else's draft, then the action is refused.

**TT-14 Track my requests and their status**
As a Manager, I want a list of my department's requests with status and current step, so that I don't have to chase approvals by email.
- Given requests exist, when I open "My requests", then I see reference, type, shift date, cost, status and current approver role, newest first.
- Given the list, when I filter by status, then only matching requests are shown.
- Given a request in the list, when I open it, then I see all fields, the cost breakdown and the approval chain with each step's state.

**TT-15 Route submitted request through approval chain**
As a Finance approver, I want Agency and high-cost requests to include a Finance step, so that significant spend gets financial review.
- Given an Overtime or Bank request with cost below the threshold, when it is submitted, then its chain is Head of Nursing → CNO.
- Given an Agency request, when it is submitted, then its chain is Head of Nursing → CNO → Finance.
- Given any request with cost equal to or above the threshold, when it is submitted, then Finance is added as the last step.
- Given the chains and threshold are changed in seed data, when the app restarts, then new submissions use the new rules without code changes.

**TT-16 View my pending approvals queue**
As a Head of Nursing, I want a queue of requests waiting for my decision, so that I can act on them quickly.
- Given requests are pending at a step for my role and scope, when I open "Approvals", then I see them with reference, department, type, shift date, cost and submitted date, oldest first.
- Given a request is pending at another role's step, when I open my queue, then it is not shown.
- Given I raised a request myself, when it reaches a step for my role, then it is not in my queue.

**TT-17 Approve a request step**
As an approver, I want to approve the step assigned to me with an optional comment, so that the request moves on.
- Given a request is pending at my step, when I approve, then it moves to the next step, or to `Approved` if mine was the last.
- Given a request is not at my step, when I call approve via the API, then the action is refused and nothing changes.
- Given two approvers act on the same step at once, when both submit, then only one decision is recorded and the other gets a clear conflict message.

**TT-18 Reject a request with a reason**
As an approver, I want to reject a request with a reason, so that the requester knows what to change.
- Given a request is pending at my step, when I reject with a reason of at least 5 characters, then it becomes `Rejected` and the reason is shown on the request.
- Given I leave the reason empty, when I reject, then I see a validation error and the request is unchanged.

**TT-19 Revise and resubmit a rejected request**
As a Manager, I want to edit a rejected request and resubmit it, so that I don't have to start again.
- Given my request is `Rejected`, when I open it, then I see the rejection reason and can edit it.
- Given I edit and resubmit, when the request is saved, then cost and chain are recalculated and it returns to `Pending` at step 1.
- Given a resubmitted request, when I view its history, then the earlier rejection is still visible.

**TT-20 Record audit events for every request action**
As a CNO, I want every action on a request recorded with who, when and why, so that decisions are accountable.
- Given any create, update, submit, approve, reject, resubmit or export action, when it succeeds, then an audit event is stored with UTC timestamp, actor, role, step and comment.
- Given an update, when it is audited, then the event lists the changed fields with old and new values.
- Given an action fails validation, when it is rejected, then no audit event is written for it.
- Given the API, when I look for a way to change or delete audit events, then none exists.

**TT-21 View request audit history**
As a Finance approver, I want to see a request's full history on its detail page, so that I can check how it was approved.
- Given a request I am allowed to see, when I open its detail page, then I see its audit events in time order with local time, actor name, role, action and comment.
- Given a request I am not allowed to see, when I request its history via the API, then I get not found.

**TT-22 Write workflow notifications to an outbox log**
As an Admin, I want each workflow notification written to an outbox I can view, so that we can show notification triggers without an email service.
- Given a request is submitted or resubmitted, when the transition succeeds, then an outbox entry addressed to the step-1 approvers is written.
- Given a step is approved and another step follows, when it succeeds, then an entry addressed to the next-step approvers is written.
- Given a request is finally approved or rejected, when it succeeds, then an entry addressed to the requester is written.
- Given I am Admin, when I open "Outbox", then I see entries with event, recipients, subject and time, newest first.

**TT-23 Export approved requests to Excel with filters**
As a Finance user, I want to export approved requests to Excel filtered by period, department and staff type, so that I can feed existing finance reports.
- Given I am Finance, CNO or Admin, when I choose a shift-date range, optional department and optional staff type and export, then I download an `.xlsx` with one row per approved request matching the filters.
- Given the file, when I open it in Excel, then it has a header row and columns: reference, department, staff type, band, shift date, start, end, paid hours, headcount, hourly rate, total cost, requester, submitted at, approved at, approvers.
- Given no requests match, when I export, then I get a file with only the header row and a message "0 rows".
- Given I am a Manager or Head of Nursing, when I call the export API, then it is refused **(assumption, Q15)**.

## 7. PoC demo scenario (smoke test basis)
1. Sign in as Manager (Ward A). Create a Bank request below the threshold, see cost, submit.
2. Sign in as Head of Nursing, approve. Sign in as CNO, approve. Request is `Approved`.
3. As Manager, create an Agency request, submit. Head of Nursing rejects with a reason.
4. Manager revises and resubmits. Head of Nursing, CNO, Finance approve.
5. Sign in as Manager (Ward B): neither request is visible.
6. Open each request's audit history; open the Admin outbox.
7. As Finance, export approved requests for the period to Excel.

## 8. Dependencies and risks
- Client design (presentation PDF and `TempTrack-Demo.html` referenced in the PRD) is not in the repo (Q7).
- Final approval flow is TBD by the client (Q2); the PoC keeps chains in seed data so a change is a data edit.
- PRD 3.2 and the Q&A disagree on approvers (Manager → Directorate Lead → Finance vs Head of Nursing → CNO).

## 9. Clarifying questions

None blocks the PoC kickoff if the Tech Lead accepts the proposed defaults. Q1, Q2, Q3, Q5 and Q7 shape the demo most
and should be confirmed (or the default accepted) at the kickoff gate. All others are needed before a full build.

| # | Question | Why it matters | Proposed default |
| --- | --- | --- | --- |
| 1 | Azure AD / Entra ID: is a tenant and app registration available, and do roles come from AD groups or from TempTrack? | Shapes the auth and user model; the PoC uses mock auth | PoC: mock auth with role switching. Full: Entra ID with app roles mapped from AD groups |
| 2 | What is the final approval chain? PRD 3.2 says Manager → Directorate Lead → Finance; the Q&A says Head of Nursing → CNO (2–3 roles). | Core of the workflow and demo | Head of Nursing → CNO; Finance added for Agency and high-cost (seed data) |
| 3 | What is the Agency "manual approval" step: who performs it, and is it inside TempTrack? | Defines the third step for Agency | A Finance-role step in TempTrack labelled "Agency approval" |
| 4 | What is the high-cost threshold, and is it per request, per shift or per period? | Drives routing | GBP 1,000 per request total (placeholder) |
| 5 | Cost formula: are there night/weekend/bank-holiday enhancements, overtime multipliers, agency commission or VAT? Is the rate keyed by band, role or both? | Cost shown must match finance expectations | Flat `headcount × paid hours × rate(type, band)`; no enhancements |
| 6 | Currency and source of pay rates; can we get anonymised rate structures (not values) for the seed? | Realistic demo | GBP, synthetic rates |
| 7 | Can we get the client design (presentation PDF, `TempTrack-Demo.html`) and the required request fields and reason codes? | UI and data model | Fields as in 4.1; plain accessible UI, design applied after the PoC |
| 8 | Is a request one shift, or can it cover several dates/shifts? | Data model and cost | One shift on one date × headcount |
| 9 | Organisation: departments, directorates, which Head of Nursing covers which departments; does a Deputy CNO act with CNO rights? | Routing and visibility | 4 synthetic departments in 2 directorates; Deputy CNO = CNO role; delegation deferred |
| 10 | Visibility: who sees which requests (department-level per PRD 3.6)? | RBAC | Manager: own department; HoN: scoped departments; CNO/Finance/Admin: all |
| 11 | After rejection and resubmission, does approval restart at step 1 or at the rejecting step? | Workflow rule | Restart at step 1 |
| 12 | Can a requester withdraw or edit a request while pending? | States and audit | No in PoC; withdraw deferred |
| 13 | What does "escalation" mean: only threshold-based extra steps, or also time-based reminders/escalation to another approver? With what timings? | PRD 3.2/3.3 mention escalation without rules | PoC: threshold-based only; time-based deferred |
| 14 | Which email service (unanswered in the PRD Q&A): client SMTP relay, Microsoft 365 / Graph, or other? Are "real-time notifications" (team section) in scope? | Integration and cost | PoC: outbox stub. Full: Microsoft Graph via the client's M365 |
| 15 | Excel export: client template and columns; which statuses; who may export? | Must match existing reports | Approved only; columns in TT-23; Finance, CNO, Admin |
| 16 | Data residency and hosting: team default is EU; the client is a UK hospital (UK GDPR) and the Q&A says Azure or an internal dedicated environment. Which region/host? | Compliance and Gate 2 | Decide at Gate 2; PoC runs locally on synthetic data |
| 17 | Audit retention period and who may read audit history? | Storage and access | Retain for the life of the system; anyone who can see the request sees its history |
| 18 | Expected users and request volumes per month? | Performance targets | Confirm; PoC targets in section 5 assume ~500 seeded requests |
| 19 | Supported devices: desktop only, or tablets/phones on wards? | Layout and testing | Desktop Edge/Chrome; usable at 360 px |
| 20 | Who attends the PoC demo, and what result counts as success? | Defines "done" for the week | Tech Lead + client demo of the section 7 scenario |
