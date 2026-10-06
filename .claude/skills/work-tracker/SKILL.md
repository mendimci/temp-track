---
name: work-tracker
description: Tool-agnostic work item operations (create, update, transition, link, query, comment) for epics, stories, tasks and bugs. Use whenever any agent needs to read or change work items; never call Jira or Azure DevOps directly.
---
# Work tracker (neutral interface)

1. Read `tools.tracker` from `project/project.yaml` (provider, project_key, and for external trackers url, status_map, type_map).
2. Open the adapter file for the provider: `adapters/<provider>.md`, and follow it.
   - `markdown` (default): tickets are files in `docs/backlog/`; no external tool or MCP server.
   - `jira`, `azure-devops`, `github-issues`: external trackers. Use them only after the Tech Lead has confirmed the integration and `/setup-project tracker` has connected it.
3. Always speak in canonical terms; external adapters translate them with the maps:
   - Types: `epic`, `story`, `task`, `bug`
   - States: `backlog`, `ready`, `in_progress`, `in_review`, `done`

## Operations
| Operation | Inputs | Returns |
| --- | --- | --- |
| create | type, title, description, parent?, labels? | key |
| update | key, fields | ok |
| transition | key, canonical state | ok |
| link | key, other key or PR URL | ok |
| query | canonical state(s), labels? | list of key, title, state |
| comment | key, text | ok |

## Rules
- Descriptions use the templates in the `write-story` skill.
- Every PR must be linked to its item.
- If an external provider's MCP server is not connected, stop and tell the Orchestrator to run `/setup-project tracker`.
