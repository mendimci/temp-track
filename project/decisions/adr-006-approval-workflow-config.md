---
status: accepted   # proposed | accepted | superseded
date: 2026-10-06
---
# ADR-006: Approval workflow: data-driven chains and routing rules, hand-written state machine

## Context
The PRD asks for a "configurable, rule-based multi-step approval workflow". The final chain is not known (Q2; PRD
and Q&A disagree). The PoC needs Head of Nursing → CNO, plus Finance for Agency or cost ≥ threshold, changeable without
code (TT-15). There are only four states (Draft, Pending(k), Approved, Rejected). Admin UI is deferred.

## Decision
- Store **chains** (`approval_chain`, `approval_chain_step`) and **routing rules** (`routing_rule`: priority, optional
  staff type, optional minimum cost, chain) as data, loaded from `seed/*.json` and **upserted by natural key at
  startup**. Seed default: Agency → extended; cost ≥ 1000.00 → extended; else standard.
- At submit/resubmit, `RoutingPolicy` picks the first matching rule and **snapshots** the chain into `request_step`
  rows for that round. In-flight requests are not affected by later rule changes.
- The lifecycle is a small hand-written state machine in the `StaffingRequest` aggregate with explicit guards;
  optimistic concurrency on the request row prevents double decisions.
- Resubmission restarts at step 1 (Q11 default) by creating a new round; earlier rounds stay for history.

## Alternatives considered
| Option | Pros | Cons |
| --- | --- | --- |
| **Rules and chains as data + simple state machine (chosen)** | Change chain/threshold by editing seed (admin UI later edits the same tables); tiny, testable code | Rule language is deliberately limited (type, min cost) |
| Hard-coded chains in C# | Simplest code | Every client change needs a release; fails TT-15 |
| Workflow engine (Elsa 3, Workflow Core) | Visual designer, timers for escalation | Heavy dependency and learning curve for four states; persistence model of its own; overkill for the PoC |

## Consequences
- Q2/Q3/Q4 answers become seed edits, not code changes.
- Time-based escalation (Q13) would need a scheduled job later; the step table already has the timestamps it needs.
- The admin UI (deferred) edits these tables and must record changes (PRD 3.7 change tracking).
