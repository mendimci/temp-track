---
status: accepted   # proposed | accepted | superseded
date: 2026-10-06
---
# ADR-002: Hosting: local Docker Compose for the PoC, Docker images on Portainer later

## Context
PoC mode: one week, runs locally on synthetic data; deployment is deferred (requirements §2.3). `project.yaml`
targets the company Portainer for dev, staging and production, with separate app and API URLs. The PRD Q&A says
"Azure, but hosting can be in a dedicated environment internally"; PRD §7 makes infrastructure and running costs a
client matter. The client is a UK hospital (UK GDPR); the team default is EU residency.

## Decision
- **PoC:** `docker compose` with three services: `db` (postgres:17), `api` (.NET 10 runtime image), `web` (nginx serving
  the built SPA with a runtime `/config.json`). Developers may run `api` and `web` natively against the compose `db`.
- **Later:** the same two application images (built once in CI, pushed to a registry) are deployed per environment as a
  Portainer stack behind a TLS reverse proxy, triggered by the existing webhooks. Images contain no environment config
  (12-factor), so moving production to Azure (App Service or Container Apps + Azure Database for PostgreSQL) needs no
  code change.
- No cloud resources are created for the PoC.

## Alternatives considered
| Option | Pros | Cons |
| --- | --- | --- |
| **Local compose now, Portainer later (chosen)** | Zero cost for the PoC; matches `project.yaml`; portable images | Production location and residency still open |
| Azure PaaS from day one (App Service + PostgreSQL Flexible, UK South) | UK residency; client's ecosystem; managed backups | Needs subscription, IaC and cost now; not needed to prove the core loop |
| Client's internal dedicated environment | Data stays with the client | Access, network and lead time unknown; blocks a one-week PoC |

## Consequences
- Rough running cost per month: Portainer USD 60-120 total, Azure USD 170-230 total (`docs/architecture.md` §13.1).
- The production host and region must be decided before full-build planning (Tech Lead question Q-A2).
- Mock auth must not be exposed on any deployed environment without a network guard (ADR-004).
