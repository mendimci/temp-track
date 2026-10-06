---
description: Prepare the release - QA report, SBOM report, docs, runbook - and the Gate 4 pack
---
Use the orchestrator subagent:
1. qa-engineer: final run on Staging and `docs/qa-report.md`.
2. code-reviewer: `docs/sbom-report.md` from the latest CI SBOM.
3. devops-engineer: confirm `docs/runbook.md` and rollback steps.
4. tech-writer: README, user guide, `docs/handover.md`.
5. Write `project/decisions/gate-4.md` and stop. Production deploy happens only via the `deploy-prod` workflow after the Tech Lead approves it in GitHub.
