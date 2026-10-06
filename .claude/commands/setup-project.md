---
description: Interview the Tech Lead and create project/project.yaml (tools, environments, budget), then verify every connection
argument-hint: "[optional: tool to reconfigure, e.g. tracker]"
---
Set up this project's configuration. If `$ARGUMENTS` names one area (tracker, code_host, ci, docs, environments, budget), reconfigure only that area and keep the rest of `project/project.yaml` unchanged.

First, template cleanup (only on the first run in a new project):
- If `.template-only` exists and `gh repo view --json isTemplate --jq .isTemplate` prints `false`, delete every path listed in it.
  If it prints `true`, this is the template repo itself: skip cleanup.
- Replace the deleted `README.md` with a short project README: project name, client, PRD link, environment URLs, and the commands `/kickoff`, `/sprint`, `/status`. The tech-writer maintains it from then on.

Use `project/project.example.yaml` as the schema. Ask the Tech Lead one short group of questions at a time (use multiple-choice where possible, with defaults pre-selected):

1. Project: name, client, mode (poc | full), Tech Lead name. The PRD is `docs/prd.md` (structure: `templates/prd-template.md`); if it's missing, say so and continue, `/kickoff` needs it.
2. Tracker: default `markdown` (tickets in `docs/backlog/`, no external tool); ask only for the ticket prefix `project_key` (e.g. `LEAVE`).
   Docs: default `markdown` (docs in `docs/`, decisions in `project/decisions/`). Code host and CI: default GitHub and GitHub Actions.
3. External tools (Jira, Azure DevOps, GitHub Issues, Notion, Confluence): only if the Tech Lead explicitly confirms the integration now. Then:
   copy the needed servers from `.mcp.integrations.example.json` into `.mcp.json`, ask the Tech Lead to authenticate them with `/mcp`,
   ask for instance URL and project key, read the tracker's real workflow through its MCP server and PROPOSE `status_map` and `type_map` for confirmation.
4. Environments: dev and staging host (default Portainer on Code-invention infrastructure) and URLs; production host (client Azure/AWS/dedicated or ours).
5. Budget: project budget in USD (from the PRD/contract), provisional token cap as a percentage of it (default 5%), optional absolute `token_cap_usd` override, pause at 80%.

Secrets: list the secret NAMES needed and where to store them (GitHub → Settings → Environments, or Azure). NEVER ask for or accept token values in chat. If the user pastes a secret, tell them to revoke it.

Run `npm ci --prefix scripts` once (the backlog CLI and validator need it). Then run a connection check and print a pass/fail table:
- markdown tracker: `node scripts/backlog.mjs check`; PRD: `docs/prd.md` exists; code host: `gh repo view`.
- external tools: read one work item / call the docs page through the MCP server.
Finally write `project/project.yaml`, validate it with `node scripts/validate-project.mjs`, commit it together with the template cleanup as `chore: project setup`, and print a summary for Tech Lead sign-off.
