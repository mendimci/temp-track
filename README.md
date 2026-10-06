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

## Web SPA development

Node 22+. From `src/web`: `npm ci`, then `npm run dev` (reads `public/config.json` for `apiUrl` and `authMode`; point `apiUrl` at the Dev API).
Checks: `npm run lint`, `npm run format:check`, `npm run typecheck`, `npm test`, `npm run build`.
`npm run gen:api` regenerates `src/api/schema.d.ts` from `docs/api/openapi.yaml`.
