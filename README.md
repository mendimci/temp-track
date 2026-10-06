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
`npm run gen:api` regenerates `src/api/schema.d.ts` from `docs/api/openapi.json`.

## CI

`.github/workflows/ci.yml` runs on every PR: `build-test-api` (locked restore, warnings-as-errors build, `dotnet format --verify-no-changes`, tests with coverage), `build-test-web` (lint, format, typecheck, generated-types check, tests with coverage, build), `build-test` (aggregate gate), `sbom` and `validate-project`. Coverage is report-only in the PoC (step summary and artifacts).
Locally: `dotnet test TempTrack.slnx --collect:"XPlat Code Coverage"` (needs Docker) and `npm run test:coverage` in `src/web`.
