# TempTrack

Temporary staffing request system for **St John & St Elizabeth Hospital**.

- PRD: [docs/prd.md](docs/prd.md)
- Project settings: [project/project.yaml](project/project.yaml)
- Mode: PoC

## Environments

| Environment | App | API |
| --- | --- | --- |
| Dev | https://dev.temptrack.code-invention.com | https://api.dev.temptrack.code-invention.com |
| Staging | https://staging.temptrack.code-invention.com | https://api.staging.temptrack.code-invention.com |
| Production | Code-invention Portainer (gated, Tech Lead approval) | |

## Commands

- `/kickoff`: read the PRD, run Discovery and prepare the kickoff gate
- `/sprint`: run a build sprint
- `/status`: show phase, gates, work items, environments and token spend
