using System.ComponentModel;
using System.Text;
using SupportTicketAgent.Services;

namespace SupportTicketAgent.Tools;

/// <summary>
/// Tools the AI agent invokes to query the incident CSV data.
/// </summary>
public class IncidentTools(CsvIncidentService svc)
{
    [Description("Get all incidents. Returns a one-line summary per incident.")]
    public string GetAllIncidents()
    {
        var items = svc.GetAll();
        if (items.Count == 0) return "No incidents found.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s):");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Get full details of one incident by its number (e.g. INC3500238).")]
    public string GetIncident([Description("The incident number, e.g. INC3500238")] string incidentNumber)
    {
        var i = svc.GetByNumber(incidentNumber);
        if (i == null) return $"Incident '{incidentNumber}' not found.";

        return $"""
            Incident: {i.IncidentNumber}
            Cluster: {i.ClusterId}
            Opened: {i.OpenedAt}  Closed: {i.ClosedAt}
            Priority: {i.Priority}
            Category: {i.Category} / {i.Subcategory}
            Affected CI: {i.AffectedCi}
            Site: {i.Site}
            Caller: {i.Caller}
            Assigned Group: {i.AssignedGroup}
            Short Description: {i.ShortDescription}
            Description: {i.Description}
            Symptom Tags: {i.SymptomTags}
            Root Cause: {i.RootCause}
            Resolution Notes: {i.ResolutionNotes}
            Resolution Time (hrs): {i.ResolutionTimeHrs}
            Reopen Count: {i.ReopenCount}
            KB Article: {i.KbArticleLinked}
            Problem Record: {i.ProblemRecord}
            State: {i.State}
            """;
    }

    [Description("Search incidents by keyword. Searches description, category, subcategory, symptom tags, site, assigned group, caller, root cause, and affected CI.")]
    public string SearchIncidents([Description("The search keyword")] string query)
    {
        var items = svc.Search(query);
        if (items.Count == 0) return $"No incidents found matching '{query}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s) matching '{query}':");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Filter incidents by state (e.g. Closed, Open, In Progress).")]
    public string GetIncidentsByState([Description("The state to filter by")] string state)
    {
        var items = svc.GetByState(state);
        if (items.Count == 0) return $"No incidents with state '{state}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s) with state '{state}':");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Filter incidents by priority. Values contain a number and label, e.g. '1 - Critical', '2 - High', '3 - Medium', '4 - Low'. You can search partial matches like 'Critical' or '1'.")]
    public string GetIncidentsByPriority([Description("The priority to filter by, e.g. 'Critical', 'High', '1 - Critical'")] string priority)
    {
        var items = svc.GetByPriority(priority);
        if (items.Count == 0) return $"No incidents with priority '{priority}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s) with priority '{priority}':");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Filter incidents by site name (e.g. 'WAIO Newman', 'Perth CBD', 'Escondida Chile', 'Olympic Dam SA', 'Adelaide Office', 'Brisbane Corporate').")]
    public string GetIncidentsBySite([Description("The site name or partial match")] string site)
    {
        var items = svc.GetBySite(site);
        if (items.Count == 0) return $"No incidents at site '{site}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s) at '{site}':");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Filter incidents by category or subcategory (e.g. 'Network', 'VPN Connectivity', 'Hardware', 'Security', 'Certificate Expiry').")]
    public string GetIncidentsByCategory([Description("The category or subcategory to filter by")] string category)
    {
        var items = svc.GetByCategory(category);
        if (items.Count == 0) return $"No incidents in category '{category}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s) in category '{category}':");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Get all incidents belonging to a specific cluster ID (e.g. CLU-001). Clusters group related recurring incidents.")]
    public string GetIncidentsByCluster([Description("The cluster ID, e.g. CLU-001")] string clusterId)
    {
        var items = svc.GetByCluster(clusterId);
        if (items.Count == 0) return $"No incidents in cluster '{clusterId}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {items.Count} incident(s) in cluster '{clusterId}':");
        foreach (var i in items)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }

    [Description("Get a statistical summary of incidents: counts by priority, category, site, state, root cause, and top clusters.")]
    public string GetIncidentSummary()
    {
        var items = svc.GetAll();
        if (items.Count == 0) return "No incidents in the system.";

        string Section(string title, IEnumerable<IGrouping<string, Models.Incident>> groups) =>
            $"{title}:\n" + string.Join("\n", groups.OrderByDescending(g => g.Count()).Select(g => $"  {(string.IsNullOrEmpty(g.Key) ? "(none)" : g.Key)}: {g.Count()}"));

        var byPriority = Section("By Priority", items.GroupBy(i => i.Priority));
        var byCategory = Section("By Category", items.GroupBy(i => i.Category));
        var bySite = Section("By Site", items.GroupBy(i => i.Site));
        var byState = Section("By State", items.GroupBy(i => i.State));
        var byRootCause = Section("By Root Cause", items.GroupBy(i => i.RootCause));
        var topClusters = Section("Top Clusters", items.Where(i => !string.IsNullOrEmpty(i.ClusterId)).GroupBy(i => i.ClusterId));

        var avgResolution = items
            .Where(i => double.TryParse(i.ResolutionTimeHrs, out _))
            .Select(i => double.Parse(i.ResolutionTimeHrs))
            .DefaultIfEmpty(0)
            .Average();

        var reopened = items.Where(i => int.TryParse(i.ReopenCount, out var r) && r > 0).Count();

        return $"""
            Total incidents: {items.Count}
            Average resolution time: {avgResolution:F1} hrs
            Incidents reopened at least once: {reopened}

            {byPriority}

            {byCategory}

            {bySite}

            {byState}

            {byRootCause}

            {topClusters}
            """;
    }
}
