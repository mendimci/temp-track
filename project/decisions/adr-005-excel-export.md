---
status: accepted   # proposed | accepted | superseded
date: 2026-10-06
---
# ADR-005: Excel export library: ClosedXML

## Context
TT-23 needs an `.xlsx` with a header row and one row per approved request (up to ~1,000 rows in < 5 s), numeric money
cells and real Excel dates. Licences GPL-3.0, AGPL-3.0 and SSPL are blocked in CI (`scripts/check-licences.mjs`);
non-permissive commercial licences are also unwanted. Client templates come later (Q15).

## Decision
Use **ClosedXML** (MIT) in `Infrastructure/Excel/XlsxWriter`, behind an `IExportWriter` interface.
- ClosedXML depends on DocumentFormat.OpenXml (MIT) and SixLabors.Fonts. Fonts 1.x is Apache-2.0; 2.x+ uses the
  Six Labors Split Licence. **Pin `SixLabors.Fonts` to 1.x** with an explicit package reference.
- Set fixed column widths; do not call `AdjustToContents()` (font measuring needs fonts in Linux containers).
- Generate in memory, stream the response; never store files server-side.

## Alternatives considered
| Option | Pros | Cons |
| --- | --- | --- |
| **ClosedXML (chosen)** | MIT; simple API; widely used; can open client templates later | Transitive SixLabors.Fonts licence needs pinning |
| EPPlus | Rich features | Polyform Noncommercial / commercial licence since v5: rejected |
| DocumentFormat.OpenXml SDK directly | MIT, no extra deps, fastest | Verbose low-level code for styles and number formats |
| MiniExcel | Apache-2.0, streaming, low memory | Less control over formatting and templates |
| NPOI | Apache-2.0, also .xls | Java-port API, heavier |

## Consequences
- SBOM licence check stays green without exceptions.
- Template-based exports (Q15) can load a client `.xlsx` with ClosedXML and fill it.
- If exports grow far beyond 10k rows, switch the writer implementation to OpenXml SAX streaming behind the same interface.
