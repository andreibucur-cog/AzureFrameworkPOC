using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record WorkflowRequest(
    string Question,
    RepositoryOverview? RepositoryOverview);