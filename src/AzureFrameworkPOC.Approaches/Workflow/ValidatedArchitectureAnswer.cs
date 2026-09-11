namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record ValidatedArchitectureAnswer(
    string Content,
    int AvailableEvidenceItems,
    IReadOnlyList<string> ValidationWarnings);