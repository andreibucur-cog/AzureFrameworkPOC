namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record DraftArchitectureAnswer(
    string Question,
    string Content,
    IReadOnlyList<EvidenceItem> AvailableEvidence);