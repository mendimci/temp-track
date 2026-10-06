# Implementation Plan: AI Delivery Team in Claude Code (VS Code)

Owner: Mendim Mustafa · Version 1 · 2026-10-05
Source plan: "AI Development Team — Implementation Plan" (Claude Docs)

This repo is the **starter template**. Work through the steps in order. Every step has a goal, a prompt you can paste into the Claude panel in VS Code, and an exit check. Tick the boxes as you go.

---

## Step 0: Prerequisites (½ day)

- [x] VS Code with the **Claude Code** extension (publisher: Anthropic) installed and signed in
- [x] Git, GitHub CLI (`gh auth login`), Node.js 18+ (needed for `npx` MCP servers), Docker
- [x] Windows only: Git Bash or WSL, because the hooks in `.claude/hooks/` are bash scripts
- [x] A GitHub org/repo where you can create a template repository (personal account `mendimci` during testing)
- [ ] Later, only for external integrations: access to Jira / Azure DevOps / Notion with permission to authorize MCP access (not needed to start; see "Integrations" in Week 6+)
- [ ] A Claude subscription token for GitHub Actions: run `claude setup-token` and store the result as the repo secret `CLAUDE_CODE_OAUTH_TOKEN`, never in files. PR reviews then use the subscription and add no API cost (they share its usage limits with interactive sessions). An API key (`ANTHROPIC_API_KEY`) is the alternative for org-owned client projects.

## Step 1: Publish the template repo (½ day)

The template stays under the personal account `mendimci` while it is being tested; it moves to the `code-invention` org after the pilot (see Week 6+).

```bash
cd ai-agents-development-team-template
git init && git add .gitignore README.md && git commit -m "chore: initial commit"
gh repo create mendimci/ai-agents-development-team-template --private --source . --push
git checkout -b development && git push -u origin development
# Template files: feature branch -> PR into development; then release PR development -> main (Tech Lead merges)
gh repo edit mendimci/ai-agents-development-team-template --template
```
New repos copy only the default branch (`main`), so mark the repo as a template only after the first release PR is merged.

- [x] Open the folder in VS Code and open the Claude panel
- [x] Type `/agents`: you should see the 10 agents (orchestrator … tech-writer)
- [x] Type `/`: you should see `setup-project`, `kickoff`, `gate`, `sprint`, `qa`, `release`, `status`
- [x] `.mcp.json` is configuration only: MCP servers are connected in each project created from the template, not here.

**Exit check:** The repo is a GitHub template repository, and `/agents` and `/` list every agent and command.
Passed 2026-10-05: `mendimci/ai-agents-development-team-template` is a private template, default branch `main` (57 files).

---

## Week 1: Foundations

### 1.1 Tailor team rules to Code-invention standards
Prompt:
> Read CLAUDE.md. Interview me (one question group at a time) about Code-invention's internal engineering standards: languages we prefer, code style/linters, branching, test coverage targets, security baseline, accessibility, documentation. Then update the "Engineering standards" section of CLAUDE.md. Keep it under 60 lines.

- [x] Tech Lead reviews and commits the updated `CLAUDE.md` (reviewed 2026-10-05; committed with the initial commit in Step 1)

### 1.2 Lock the project.yaml schema
Prompt:
> Create `project/project.schema.json` (JSON Schema) from `project/project.example.yaml`. Add a small script `scripts/validate-project.mjs` that validates `project/project.yaml` against it, and add a CI step that runs it when the file exists.

- [x] Schema + validation script committed (built and tested 2026-10-05; committed with the initial commit in Step 1)

### 1.2a CI/CD workflows in the template
- [x] `ci.yml` (validate-project, build-test placeholder, sbom with Grype + licence check), `deploy-dev.yml`, `promote-staging.yml`, `deploy-prod.yml` (Gate 4 check + production environment), `claude.yml` (PR review); linted with actionlint (2026-10-05)

### 1.3 PRD and backlog in the repo (no external tools to start)
- [x] PRD is `docs/prd.md`, written from `templates/prd-template.md`; decisions stay in `project/decisions/` (2026-10-06)
- [x] Markdown tracker: tickets in `docs/backlog/items/`, board `docs/backlog.md`, CLI `scripts/backlog.mjs`, CI check; `.mcp.json` starts empty (2026-10-06)

### 1.4 Adapter contract test (markdown)
Run in the sandbox project (Week 2.1).
Prompt:
> Using the work-tracker skill, run a contract test: create an epic, a story under it, a task under the story, transition the task through every canonical state, link a PR URL, query by state, add a comment. Check the board, then delete the test items. Write the results to `docs/adapter-tests/markdown.md`.

- [ ] Markdown adapter passes

**Week 1 exit check:** CLAUDE.md, the project.yaml schema, the CI/CD workflows and the markdown PRD/backlog are committed to the template repo.

---

## Week 2: Planning agents

### 2.1 Dry run with a sample PRD
The sandbox is a separate repo, run exactly like a real project in its own VS Code window. This template repo is not touched during the dry run.

- [ ] Create a test project from the template: `gh repo create mendimci/sandbox-leave-app --template mendimci/ai-agents-development-team-template --private --clone`
- [ ] Create and push its `development` branch (new repos copy only `main`)
- [ ] Write a small sample PRD in `docs/prd.md` from `templates/prd-template.md` (e.g. "Internal leave-request app") and commit it
- [ ] Run `/setup-project` (mode: `full`, tracker `markdown`); its connection check must pass
- [ ] Run the 1.4 adapter contract test here
- [ ] Run `/kickoff`

### 2.2 Review and tune
- [ ] Read `docs/open-questions.md`, `docs/requirements.md` and `project/decisions/gate-1.md`
- [ ] Run `/gate 1 approve` (or `reject` with notes), then let the architect and planner run; review `docs/architecture.md`, the ADRs, `docs/plan.md` and the board `docs/backlog.md`
- [ ] Note every correction you had to make in `docs/metrics.md` (what, which agent, why)
- [ ] Apply the fixes in the template repo (one PR per fix into `development`, then a release PR to `main`), and pull them into the sandbox as described in the README ("Maintaining this template")

Prompt to improve the agents from your notes:
> Read docs/metrics.md. For each correction, propose a specific change to the agent file in .claude/agents/ or a skill that would have prevented it. Show the diffs and wait for my approval.

**Week 2 exit check:** Gates 1–3 are reachable with only light edits from the Tech Lead.

---

## Weeks 3–4: Build agents, CI/CD and environments

### 3.1 GitHub setup (in the sandbox repo)
- [ ] Branch protection on `main`: PR required, status checks `ci / build-test` and `ci / sbom` required, no force-push
- [ ] Environments: `dev`, `staging`, `production` (production: **Tech Lead as required reviewer**)
- [ ] Secrets: `CLAUDE_CODE_OAUTH_TOKEN`, `PORTAINER_DEV_WEBHOOK`, `PORTAINER_STAGING_WEBHOOK` (+ tracker tokens if needed)
- [ ] Install the Claude GitHub App (run `claude` in the VS Code terminal, then `/install-github-app`, or install it from github.com/apps/claude) so `.github/workflows/claude.yml` can review PRs

### 3.2 Portainer
- [ ] Create a dev stack and a staging stack in Portainer that pull `ghcr.io/<org>/<repo>:dev` and `:staging`
- [ ] Enable a stack webhook for each, and save the URLs as the secrets above

### 3.3 First sprint
- [ ] Run `/sprint 1` and watch one task go end to end: branch → PR → CI + SBOM → review → merge → Dev deploy
- [ ] Then `/sprint 2` to test two engineers in parallel (git worktrees)
- [ ] Trigger `promote-staging` manually once: `gh workflow run promote-staging.yml`
- [ ] Test the safety hooks: ask Claude to `git push --force` and to run `deploy-prod`; both must be blocked

Prompt for the DevOps agent once the stack is known:
> Use the devops-engineer subagent: replace the TODO steps in .github/workflows/ci.yml with lint, test and build for the stack in project.yaml, add a Dockerfile and a Portainer stack file, and add a /health endpoint check after deploy-dev.

**Weeks 3–4 exit check:** A work item goes from *Ready* to merged and live on Dev with no human writing code, and the SBOM is generated on every PR.

---

## Weeks 5–6: Pilot (rebuild an existing internal project)

- [ ] Pick the internal project to rebuild (only once Weeks 3–4 pass)
- [ ] Create the repo from the template; write or reuse its PRD in `docs/prd.md`; run `/setup-project` and `/kickoff`
- [ ] Track in `docs/metrics.md` for every phase: elapsed time, Tech Lead interventions (and why), token cost (use `/cost` in the Claude panel per session), defects found in QA
- [ ] Run `/qa staging`, then `/release`, then approve Gate 4 and the production deploy in GitHub
- [ ] Compare with the original project: time, cost, code quality, test coverage, SBOM findings

**Pilot exit check:** Pilot delivered; comparison and intervention log reviewed by the Tech Lead.

---

## Week 6+: Harden and scale

- [ ] Transfer the template repo from `mendimci` to the `code-invention` org (`gh repo transfer` or repo Settings) and update the `mendimci/...` references in README.md and this plan
- [ ] Fix the top 5 intervention causes (agent prompts, skills, gates)
- [ ] Set real per-project token budgets from the pilot data (replace the provisional cap)
- [ ] Integrations, each only after Tech Lead confirmation, via `/setup-project tracker` or `docs` and `.mcp.integrations.example.json`:
  - [ ] Jira: company-managed project template (Backlog → Selected for Development → In Progress → In Review → Done) and the 1.4 contract test against it
  - [ ] Notion: "PRDs" and "Decision log" databases, PRD page template from `templates/prd-template.md`
  - [ ] Azure DevOps adapter test, and an Azure Pipelines variant of the workflows
  - [ ] A migration from `docs/backlog/` to the external tracker for projects that switch mid-way
- [ ] Write `docs/operator-runbook.md`: how a Tech Lead runs a project with this team
- [ ] First client project, with a human engineer shadowing

---

## Running a project (cheat sheet)

| Step | Who | Action |
| --- | --- | --- |
| 1 | Tech Lead | `gh repo create <org>/<name> --template mendimci/ai-agents-development-team-template --private --clone`, push a `development` branch, open in VS Code |
| 2 | Tech Lead | PRD in `docs/prd.md` (from `templates/prd-template.md`) |
| 3 | Tech Lead | `/setup-project` (tools, environments, budget, secrets checklist) |
| 4 | Agents | `/kickoff`: Discovery (PoC: + Architecture + Planning) |
| 5 | Tech Lead | `/gate 1 approve` … `/gate 3 approve` (PoC: `/gate kickoff approve`) |
| 6 | Agents | `/sprint` repeatedly; Dev updates on every merge, Staging daily |
| 7 | Agents | `/qa staging`, `/release` |
| 8 | Tech Lead | `/gate 4 approve`, then approve the `deploy-prod` run in GitHub |
| any | Tech Lead | `/status` |

## What's in this template

| Path | Purpose |
| --- | --- |
| `CLAUDE.md` | Team rules, gates, standards, safety, cost |
| `.claude/agents/` | 10 subagents: orchestrator, product-analyst, solution-architect, planner, backend-engineer, frontend-engineer, code-reviewer, qa-engineer, devops-engineer, tech-writer |
| `.claude/commands/` | `/setup-project`, `/kickoff`, `/gate`, `/sprint`, `/qa`, `/release`, `/status` |
| `.claude/skills/` | Neutral interfaces (`work-tracker` + markdown (default)/Jira/Azure DevOps/GitHub adapters, `code-host`, `deploy`) and know-how (`write-story`, `write-adr`, `estimate-task`, `gate-pack`, `sbom`) |
| `.claude/hooks/` + `settings.json` | Block force-push, prod deploys, destructive commands and committed secrets; permission allow/deny lists |
| `.mcp.json`, `.mcp.integrations.example.json` | Empty by default; Atlassian, Notion, GitHub and Azure DevOps servers to copy in when an integration is enabled |
| `docs/prd.md`, `docs/backlog/`, `scripts/backlog.mjs` | PRD and markdown tracker (tickets, generated board, CLI) |
| `project/project.example.yaml` | Per-project config filled by `/setup-project` |
| `.template-only` | Template-maintenance files (this plan, the template README) that `/setup-project` removes in new projects |
| `project/project.schema.json` + `scripts/` | Schema for `project.yaml` and its validator (`node scripts/validate-project.mjs`), run in CI |
| `templates/` | Gate pack, ADR, PRD structure |
| `.github/workflows/` | CI + SBOM, Claude PR review, Dev deploy, daily Staging promotion, gated Production |

## Notes and limits

- MCP endpoints change over time. If one fails, check the vendor's docs: Atlassian `https://mcp.atlassian.com/v1/mcp`, Notion `https://mcp.notion.com/mcp`, GitHub `https://api.githubcopilot.com/mcp/`, Azure DevOps `npx -y @azure-devops/mcp <org>`.
- Agents without a `tools:` line inherit all tools, including MCP servers. Those that only need files (architect, reviewer, tech writer) are restricted.
- `deploy-prod` cannot be triggered by agents (hook + permission deny + GitHub environment reviewers).
- The CI build/test steps are placeholders until the stack is chosen at Gate 2.
