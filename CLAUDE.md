# Code-invention AI Delivery Team: Team Rules

You are part of an AI delivery team that builds software from a PRD to a released product.
A human **Tech Lead** owns every decision gate. Agents do the work between gates.

## Source of truth
- Project settings: `project/project.yaml` (mode, client, tools, environments, budget). Read it first, every session.
- Requirements: `docs/prd.md` (written by the Tech Lead from `templates/prd-template.md`) → `docs/requirements.md`.
- Architecture: `docs/architecture.md` + ADRs in `project/decisions/`.
- Work: the tracker configured in `project.yaml`. Default `markdown`: tickets in `docs/backlog/items/`, board in `docs/backlog.md`.
  External tools (Jira, Azure DevOps, GitHub Issues; Notion, Confluence) are supported but enabled per project only after Tech Lead confirmation.

## Tool-agnostic rule
Never call Jira, Azure DevOps, GitHub or a hosting provider directly from an agent prompt.
Always use the neutral skills: `work-tracker`, `code-host`, `deploy`. They read `project.yaml` and pick the adapter.

## Phases and gates
1. Discovery → **Gate 1 Scope**
2. Architecture → **Gate 2 Architecture** (stack, hosting cost, token cost)
3. Planning → **Gate 3 Plan**
4. Build sprints → Dev (on every merge to `development`) → Staging (daily, from `main`)
5. QA and hardening → **Gate 4 Release**
6. Production and handover

PoC mode (`mode: poc`): max 1 week. Gates 1–3 merge into one kickoff approval. Smoke tests only.
A gate is passed ONLY when `project/decisions/gate-<n>.md` exists with `status: approved` signed by the Tech Lead.
Never start a phase whose gate is not approved.

## Engineering standards (every PR)

### Stack and style
- Preferred stacks — backend: Node.js + TypeScript, .NET (C#), or Python; frontend: React + TypeScript or Angular.
  The Solution Architect picks among these per project; anything else needs an ADR and Tech Lead approval.
- Code style: standard tool defaults (ESLint + Prettier, `dotnet format`, Ruff), enforced in CI. No style debates in PRs.
- Comments are sparse: single-line, short, and only where they explain something the code can't say
  (a constraint, a why). Never comment what the next line does; code should be self-explanatory.

### Design principles
- KISS and YAGNI: the simplest design that meets the acceptance criteria; nothing for hypothetical future needs.
- DRY with the Rule of Three: extract shared code on the third repetition, not before.
- Single responsibility and separation of concerns: thin handlers, logic in services/domain, data access behind repositories.
- Follow existing patterns in the codebase; introducing a new pattern or architecture style needs an ADR.
- Fail fast: validate at boundaries, never swallow exceptions, return clear errors.
- 12-Factor: config from environment, stateless services, logs to stdout (structured, correlation ID, no PII).
- Composition over inheritance; inject external dependencies (DB, HTTP, clock) so they can be mocked.
- Idempotent migrations, deploy scripts and retried operations.
- Tests follow the pyramid and Arrange–Act–Assert; test behaviour, not implementation.

### Branching and commits
- Trunk-based off `development`: one task = one branch = one PR into `development`, linked to its work item.
  Branch: `<type>/<ticket-key>-<slug>`.
- `main` is release-only: a release PR `development` → `main`, approved by the Tech Lead.
  Dev deploys from `development`; Staging and Production from `main`.
- Conventional Commits. Small PRs (aim < 400 changed lines).

### Testing
- Tests added for new behaviour; CI green; Reviewer agent approved.
- Coverage ≥ 80% on new/changed lines (CI gate); no retroactive target for existing code.
- Full mode requires all levels: unit, integration/API, E2E (Playwright) on critical paths, OpenAPI contract tests.
  PoC mode: smoke tests only.

### Security and data
- OWASP Top 10 review on every PR. SBOM in CI; no blocked licences (GPL-3.0, AGPL-3.0, SSPL unless approved
  by ADR) and no critical CVEs.
- No secrets in code, prompts or logs. Secrets live in GitHub/Azure environment secrets only.
- GDPR: EU data residency by default, PII inventory in `docs/architecture.md`, synthetic data outside
  production (`data_rules.client_data_allowed`).

### Accessibility and docs
- Frontends meet WCAG 2.1 AA (semantic HTML, keyboard navigation, contrast), checked in review and by
  automated axe/Playwright checks.
- Mandatory docs: README + `docs/runbook.md`, ADRs for key choices, API docs from the OpenAPI spec, and at
  release `docs/user-guide.md` + `docs/handover.md`. Update docs when behaviour changes.

## Safety
- Never deploy to production. Only the `deploy-prod` workflow does that, after Tech Lead approval.
- Never force-push, never rewrite `main` or `development` history, never disable CI checks.
- Agents never merge `development` into `main`; the release PR is opened by the Orchestrator and merged by the Tech Lead.
- If stuck after 3 attempts on the same problem, stop and escalate to the Orchestrator with a short summary.

## Cost
- Respect the token budget in `project.yaml`: initially `budget.token_cap_percent` of the project
  budget (`budget.project_budget_usd`). After the pilot, projects switch to an absolute cap
  (`budget.token_cap_usd`); if both are set, the USD cap wins.
- At `budget.pause_at_percent` (default 80%) of the cap the Orchestrator pauses and asks the Tech Lead.
- Agents and CI run on the Claude subscription, so spend is measured at API-equivalent prices (`/cost`);
  the cap is a usage guardrail, not a bill.
- Use the cheapest capable model: planning/review on Opus, building on Sonnet, docs on Haiku.

## Communication
- Only the Orchestrator talks to the human. Other agents hand off through files and work items.
- Write short, factual updates. Record decisions as ADRs.
