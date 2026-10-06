---
status: proposed   # proposed | accepted | superseded
date: 2026-10-06
---
# ADR-004: Authentication: flag-guarded mock now, Entra ID later, behind one abstraction

## Context
The PRD requires Azure AD / Entra ID SSO with role-based, department-level access. The PoC defers SSO (Q1) and needs
fast role switching for the demo (TT-9). The NFRs require mock auth to run only with an explicit flag and the app to
refuse to start otherwise. App and API run on separate origins (`project.yaml`).

## Decision
- **Pluggable scheme**, selected by `Auth__Mode` (`Mock` | `Entra`, no default):
  - **Mock (PoC):** SPA sends `X-Dev-User: <userId>` chosen from `GET /api/dev-auth/users`; a custom
    `AuthenticationHandler` loads the seeded user. Startup throws unless `Auth__MockEnabled=true` and the environment
    is not `Production`. The dev-auth endpoints are only mapped in Mock mode.
  - **Entra (later):** SPA uses `@azure/msal-browser`/`msal-react` (auth code + PKCE) and sends a bearer token; API
    uses Microsoft.Identity.Web for JWT validation; claims transformation maps `oid` to `app_user.external_id`.
- Both produce the same principal (`tt:user_id`, `role`). All authorisation uses `ICurrentUser` + `VisibilityPolicy`
  + named policies, never raw claims. SPA uses an `AuthProvider` interface (`MockAuthProvider` now, `MsalAuthProvider`
  later).
- Department scope stays in the TempTrack database in both modes; the role source under Entra (app roles via groups
  vs TempTrack DB) is decided with the client (Q1).
- **Real Entra in the PoC:** possible cheaply (company Entra tenant, one app registration with app roles, 5 cloud-only
  test users, Entra ID Free; about 0.5-1 day). Not recommended for the demo: each role switch becomes a full
  sign-out/sign-in with MFA, slowing the demo, and it consumes a day of a five-day timebox. Recommended as an optional
  spike on day 5 only if the core loop is done.

## Alternatives considered
| Option | Pros | Cons |
| --- | --- | --- |
| **Header-based mock + Entra bearer later (chosen)** | Instant role switching; no cookies/CSRF; same request shape as bearer tokens, so the swap is local | Header is trivially spoofable, so it must never be reachable unguarded |
| Real Entra ID in the PoC | Proves SSO early; removes a later risk | Needs tenant/app registration and test accounts; slow role switching; costs ~1 day |
| Cookie session (BFF) for both modes | Tokens never in the browser | Cross-origin cookies and CSRF handling with separate app/API URLs; more server code |

## Consequences
- Swapping to Entra touches only the auth handler registration, the claims transformation and the SPA provider.
- Smoke test: app refuses to start in Mock mode without the flag.
- Any deployed environment using Mock mode needs an IP allow-list or proxy basic auth (risk R2).
