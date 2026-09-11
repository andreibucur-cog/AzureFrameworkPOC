using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record SearchOperationResult(
    SearchOperation Operation,
    IReadOnlyList<CodeSearchResult> Matches);