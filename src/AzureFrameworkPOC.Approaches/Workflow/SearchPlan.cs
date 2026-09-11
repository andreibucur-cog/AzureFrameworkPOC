using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record SearchPlan(
    string Question,
    RepositoryOverview RepositoryOverview,
    IReadOnlyList<SearchOperation> Operations);