---
description: Run QA against Dev or Staging, file bugs, and update the QA report
argument-hint: "[dev|staging]"
---
Delegate to the qa-engineer subagent for environment `$ARGUMENTS` (staging if empty). Afterwards summarise: tests run, pass rate, new bugs by severity, and whether the release criteria in `docs/qa-report.md` are met.
