---
name: sbom
description: Generate and check the Software Bill of Materials (CycloneDX) and licence/vulnerability status. Use in CI setup, PR review and release.
---
- CI generates `sbom.cdx.json` (CycloneDX) with Syft on every PR and uploads it as an artifact; Grype scans it for vulnerabilities.
- Local check: `syft dir:. -o cyclonedx-json > sbom.cdx.json` then `grype sbom:sbom.cdx.json --fail-on critical`.
- Blocked licences by default: GPL-3.0, AGPL-3.0, SSPL. CI checks them with `node scripts/check-licences.mjs sbom.cdx.json`.
  Override only by ADR, then add the package name and ADR number to `.github/licence-exceptions.txt`.
- Release: attach the SBOM to the GitHub release and summarise it in `docs/sbom-report.md` (components, licences, open CVEs with status).
