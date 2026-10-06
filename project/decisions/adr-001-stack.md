---
status: accepted   # proposed | accepted | superseded
date: 2026-10-06
---
# ADR-001: Stack: .NET 10 API + React/TypeScript SPA

## Context
TempTrack needs a REST API with a small workflow, relational data, Excel export and (later) Entra ID SSO, plus an
accessible web UI. The client asked for ".NET for the backend, React in the frontend" (PRD Q&A) and runs on
Microsoft/Azure. Both are on the team's preferred-stack list (CLAUDE.md). PoC is one week; the team must be able to
maintain the result after handover.

## Decision
- **Backend:** .NET 10 (LTS, supported to Nov 2028), ASP.NET Core minimal APIs grouped per feature, EF Core 10 with
  Npgsql, built-in `Microsoft.AspNetCore.OpenApi` to generate the OpenAPI document from code, built-in JSON console
  logging. One project with `Domain/`, `Features/`, `Infrastructure/` folders (layout in `docs/architecture.md` §10).
- **Frontend:** React 19 + TypeScript (strict), Vite, React Router, TanStack Query, `openapi-typescript` +
  `openapi-fetch` for a typed client generated from `docs/api/openapi.json`, React Hook Form. Plain CSS, no component
  library (accessibility from semantic HTML).
- **Tests:** xUnit v3, Testcontainers (PostgreSQL), Playwright + `@axe-core/playwright`.
- **Style:** `dotnet format`, ESLint + Prettier, enforced in CI.

## Alternatives considered
| Option | Pros | Cons |
| --- | --- | --- |
| **.NET 10 + React (chosen)** | Client's stated preference; first-class Entra ID (Microsoft.Identity.Web) and Azure support; strong typing end to end; mature EF Core; LTS | Two languages in the repo |
| Node.js + TypeScript (NestJS) + React | One language across the stack; fast iteration | Contradicts the client's stated .NET preference; Entra integration less turnkey; weaker fit for the client's Microsoft IT |
| Python (FastAPI) + React | Quick to write; good OpenAPI generation | Contradicts client preference; Decimal/typing discipline weaker; Excel and Entra libraries less integrated |
| .NET + Angular (frontend variant) | Opinionated, batteries included | Client asked for React; heavier for a one-week PoC |

## Consequences
- The client's IT can operate and extend a familiar Microsoft stack; Entra drop-in is low risk (ADR-004).
- The OpenAPI spec is an artefact of the API build, not hand-written; the SPA types regenerate from it, so contract
  drift shows up as a TypeScript compile error.
- `Domain/` must stay free of EF Core and ASP.NET references (review check) so the single project can be split later.
- CI needs both the .NET 10 SDK and Node 22.
