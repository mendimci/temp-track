---
name: write-story
description: Format for epics, user stories, acceptance criteria and bug reports. Use when writing requirements or creating work items.
---
**Story**
```
Title: <verb> <object> (max 10 words)
As a <persona>, I want <goal>, so that <benefit>.
Acceptance criteria:
- Given <context>, when <action>, then <result>.
Notes: links, designs, constraints
Definition of done: tests, docs, reviewed, deployed to Dev
```
**Bug**
```
Title: <what is broken> on <where>
Environment: dev | staging; version/commit
Steps to reproduce: 1. 2. 3.
Expected / Actual
Severity: critical | major | minor
Linked story: <key>
```
Stories must be independent, negotiable, valuable, estimable, small and testable (INVEST).
