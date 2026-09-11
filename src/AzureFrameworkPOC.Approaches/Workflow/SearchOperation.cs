namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record SearchOperation(
    string SearchTerm,
    string? FilePattern,
    string Purpose);