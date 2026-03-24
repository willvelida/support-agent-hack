using SupportTicketAgent.Models;

namespace SupportTicketAgent.Services;

public class CsvIncidentService
{
    private readonly string _csvPath;
    private readonly Lock _lock = new();

    public CsvIncidentService(string csvPath)
    {
        _csvPath = csvPath;
    }

    public List<Incident> GetAll()
    {
        lock (_lock)
        {
            return ReadIncidents();
        }
    }

    public Incident? GetByNumber(string incidentNumber)
    {
        return GetAll().FirstOrDefault(i =>
            i.IncidentNumber.Equals(incidentNumber, StringComparison.OrdinalIgnoreCase));
    }

    public List<Incident> Search(string query)
    {
        return GetAll().Where(i =>
            i.ShortDescription.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.Subcategory.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.SymptomTags.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.Site.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.AssignedGroup.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.Caller.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.RootCause.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.AffectedCi.Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    public List<Incident> GetByState(string state) =>
        GetAll().Where(i => i.State.Equals(state, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Incident> GetByPriority(string priority) =>
        GetAll().Where(i => i.Priority.Contains(priority, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Incident> GetBySite(string site) =>
        GetAll().Where(i => i.Site.Contains(site, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Incident> GetByCategory(string category) =>
        GetAll().Where(i =>
            i.Category.Contains(category, StringComparison.OrdinalIgnoreCase) ||
            i.Subcategory.Contains(category, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Incident> GetByCluster(string clusterId) =>
        GetAll().Where(i => i.ClusterId.Equals(clusterId, StringComparison.OrdinalIgnoreCase)).ToList();

    private List<Incident> ReadIncidents()
    {
        if (!File.Exists(_csvPath))
            return [];

        var lines = File.ReadAllLines(_csvPath);
        if (lines.Length <= 1) return [];

        return lines.Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(ParseCsvLine)
            .ToList();
    }

    private static Incident ParseCsvLine(string line)
    {
        var f = ParseCsvFields(line);
        return new Incident
        {
            IncidentNumber = G(f, 0),
            ClusterId = G(f, 1),
            OpenedAt = G(f, 2),
            ClosedAt = G(f, 3),
            Priority = G(f, 4),
            Category = G(f, 5),
            Subcategory = G(f, 6),
            AffectedCi = G(f, 7),
            Site = G(f, 8),
            Caller = G(f, 9),
            AssignedGroup = G(f, 10),
            ShortDescription = G(f, 11),
            Description = G(f, 12),
            SymptomTags = G(f, 13),
            RootCause = G(f, 14),
            ResolutionNotes = G(f, 15),
            ResolutionTimeHrs = G(f, 16),
            ReopenCount = G(f, 17),
            KbArticleLinked = G(f, 18),
            ProblemRecord = G(f, 19),
            State = G(f, 20)
        };
    }

    private static string G(List<string> fields, int index) =>
        index < fields.Count ? fields[index].Trim() : string.Empty;

    private static List<string> ParseCsvFields(string line)
    {
        var fields = new List<string>();
        var current = "";
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        fields.Add(current);
        return fields;
    }
}
