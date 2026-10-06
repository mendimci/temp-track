# Azure DevOps Boards adapter (`azure-devops` MCP server, package @azure-devops/mcp)
- Organization from the MCP server args; project: `tools.tracker.project_key`.
- Typical type_map: epic → Epic, story → User Story (Agile) or Product Backlog Item (Scrum), task → Task, bug → Bug.
- create → create a work item; set parent link for stories under epics.
- transition → update `System.State` to `status_map[<state>]`.
- query → WIQL: `SELECT [System.Id],[System.Title],[System.State] FROM WorkItems WHERE [System.TeamProject] = '<project>' AND [System.State] IN ('<mapped>') ORDER BY [Microsoft.VSTS.Common.StackRank]`.
- link → add an artifact link to the PR (Azure Repos) or a hyperlink (GitHub PR).
- Use `AB#<id>` in commit messages/PR descriptions when code is on GitHub with the Azure Boards app.
