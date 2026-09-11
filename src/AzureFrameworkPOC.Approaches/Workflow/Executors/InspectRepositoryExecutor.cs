using AzureFrameworkPOC.Approaches.Workflow;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow.Executors;

public sealed class InspectRepositoryExecutor(
    IRepositoryInventory repositoryInventory)
    : Executor<WorkflowRequest, WorkflowRequest>(
        nameof(InspectRepositoryExecutor))
{
    public override async ValueTask<WorkflowRequest> HandleAsync(
        WorkflowRequest message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            message.Question);

        RepositoryOverview overview =
            await repositoryInventory.InspectAsync(
                cancellationToken);

        return message with
        {
            RepositoryOverview = overview
        };
    }
}