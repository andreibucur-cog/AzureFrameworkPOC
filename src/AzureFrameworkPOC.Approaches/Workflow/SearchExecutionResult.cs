using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record SearchExecutionResult(
    string Question,
    RepositoryOverview RepositoryOverview,
    IReadOnlyList<SearchOperationResult> Searches);
