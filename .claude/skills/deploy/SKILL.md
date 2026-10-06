---
name: deploy
description: Tool-agnostic deployment to Dev and Staging (Portainer by default, or client Azure/AWS/dedicated). Use when checking or triggering a non-production deploy. Never deploys production.
---
# Deploy (neutral interface)
Read `environments` from `project/project.yaml`.

- Portainer (default): deployments run in CI by calling the stack webhook stored in secret `PORTAINER_<ENV>_WEBHOOK` after pushing the image to the registry. Dev deploys on every merge to `development` (`deploy-dev.yml`). Staging deploys `main` daily (`promote-staging.yml`); agents can trigger it early with `gh workflow run promote-staging.yml`.
- Azure / AWS / dedicated: use the workflow in `.github/workflows/` that the devops-engineer created for that host; IaC lives in `infra/`.
- Verify after deploy: call `<env url>/health` and report the version.
- Production: refuse. Tell the Orchestrator that Gate 4 approval and the `deploy-prod` workflow (with required reviewers) are needed.
