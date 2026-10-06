---
status: accepted   # proposed | accepted | superseded
date: 2026-10-06
---
# ADR-003: Data store: PostgreSQL 17

## Context
TempTrack data is relational and transactional (requests, step instances, audit, outbox in one transaction), needs
exact decimal money, optimistic concurrency for simultaneous approvals and an append-only audit table. The PoC runs
locally in Docker; production is Portainer (Docker) or, alternatively, Azure. The hospital is a Microsoft shop.

## Decision
Use **PostgreSQL 17** through EF Core 10 + Npgsql (PostgreSQL licence, permissive).
- `numeric` for money, `timestamptz` (UTC) for instants, `jsonb` for audit change sets and outbox recipients.
- `xmin` system column as the EF row-version for optimistic concurrency (409 on conflict).
- A trigger rejects `UPDATE`/`DELETE` on `audit_event`.
- EF Core migrations; applied on startup in the PoC, via migration bundle in deployments later.
- Provider-specific features limited to the three above, so a switch to SQL Server stays a bounded task.

## Alternatives considered
| Option | Pros | Cons |
| --- | --- | --- |
| **PostgreSQL 17 (chosen)** | Free in every environment (incl. production containers on Portainer); small image, runs natively on ARM and x86 laptops; first-class managed option on Azure (Flexible Server, UK South); excellent EF Core provider | Less familiar to a Microsoft-centric client DBA team |
| SQL Server 2022 (container) / Azure SQL | Familiar to the hospital's Microsoft IT; Azure SQL is a natural PaaS fit | Container needs ~2 GB RAM and no native ARM image; production in a container needs a licence (Express capped at 10 GB, Developer not for production); higher Azure cost tier for equivalent size |
| SQLite | Zero setup | No real concurrency control or triggers parity with production; not a production path |

## Consequences
- Local and Portainer production cost: USD 0 licence. Azure option: PostgreSQL Flexible Server (§13.1).
- If the client mandates SQL Server (Tech Lead question Q-A4), switching costs about 1-2 days: change provider,
  regenerate migrations, replace `xmin` with `rowversion`, rewrite the audit trigger in T-SQL.
- Backups (nightly `pg_dump` or managed backups) are designed with deployment, not in the PoC.
