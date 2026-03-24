namespace SupportTicketAgent.Models;

public class Incident
{
    public string IncidentNumber { get; set; } = string.Empty;
    public string ClusterId { get; set; } = string.Empty;
    public string OpenedAt { get; set; } = string.Empty;
    public string ClosedAt { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;
    public string AffectedCi { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string Caller { get; set; } = string.Empty;
    public string AssignedGroup { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SymptomTags { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public string ResolutionNotes { get; set; } = string.Empty;
    public string ResolutionTimeHrs { get; set; } = string.Empty;
    public string ReopenCount { get; set; } = string.Empty;
    public string KbArticleLinked { get; set; } = string.Empty;
    public string ProblemRecord { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public override string ToString() =>
        $"[{IncidentNumber}] {ShortDescription} | Priority: {Priority} | {Category}/{Subcategory} | Site: {Site} | Group: {AssignedGroup} | State: {State}";
}
