using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record CollectedEvidence(
    string Question,
    RepositoryOverview RepositoryOverview,
    IReadOnlyList<EvidenceItem> Evidence);