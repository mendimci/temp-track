# Jira adapter (Atlassian Rovo MCP server, `atlassian` in .mcp.json)
- Site: `tools.tracker.url`; project: `tools.tracker.project_key`.
- create → create a Jira issue with issue type from `type_map`; stories under an epic use the epic as parent.
- transition → look up the available transitions for the issue and pick the one whose target status equals `status_map[<state>]`.
- query → JQL: `project = <KEY> AND status in ("<mapped>") ORDER BY rank`.
- link → add a remote link (PR URL) or issue link "relates to".
- Use issue keys like `EX-123` in branch names and PR titles.
