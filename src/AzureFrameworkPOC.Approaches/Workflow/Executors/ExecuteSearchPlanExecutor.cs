using AzureFrameworkPOC.Approaches.Workflow;
using AzureFrameworkPOC.Core.Exploration;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow.Executors;

public sealed class ExecuteSearchPlanExecutor(
    IRepositorySearch repositorySearch,
    WorkflowLimits limits)
    : Executor<SearchPlan, SearchExecutionResult>(
        nameof(ExecuteSearchPlanExecutor))
{
    public override async ValueTask<SearchExecutionResult> HandleAsync(
        SearchPlan message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var results =
            new List<SearchOperationResult>();

        foreach (SearchOperation operation in
                 message.Operations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<CodeSearchResult> matches =
                await repositorySearch.SearchAsync(
                    operation.SearchTerm,
                    operation.FilePattern,
                    cancellationToken);

            results.Add(
                new SearchOperationResult(
                    operation,
                    matches
                        .Take(limits.MaximumMatchesPerOperation)
                        .ToList()));
        }

        return new SearchExecutionResult(
            message.Question,
            message.RepositoryOverview,
            results);
    }
}
            