using AzureFrameworkPOC.Approaches.Workflow.Executors;
using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed class ArchitectureWorkflowFactory(
    IRepositoryInventory repositoryInventory,
    IRepositorySearch repositorySearch,
    ISourceFileReader sourceFileReader,
    SearchPlanningAgentFactory planningAgentFactory,
    SynthesisAgentFactory synthesisAgentFactory,
    WorkflowLimits limits)
{
    public async Task<Microsoft.Agents.AI.Workflows.Workflow>
        CreateAsync(
            ChatProviderType providerType,
            CancellationToken cancellationToken = default)
    {
        AIAgent planningAgent =
            await planningAgentFactory.CreateAsync(
                providerType,
                cancellationToken);

        AIAgent synthesisAgent =
            await synthesisAgentFactory.CreateAsync(
                providerType,
                cancellationToken);

        var inspect =
            new InspectRepositoryExecutor(
                repositoryInventory);

        var plan =
            new CreateSearchPlanExecutor(
                planningAgent,
                limits);

        var search =
            new ExecuteSearchPlanExecutor(
                repositorySearch,
                limits);

        var collect =
            new CollectEvidenceExecutor(
                sourceFileReader,
                limits);

        var synthesize =
            new SynthesizeAnswerExecutor(
                synthesisAgent);

        var validate =
            new ValidateAnswerExecutor();

        var builder =
            new WorkflowBuilder(inspect);

        builder.AddEdge(inspect, plan);
        builder.AddEdge(plan, search);
        builder.AddEdge(search, collect);
        builder.AddEdge(collect, synthesize);

        builder
            .AddEdge(synthesize, validate)
            .WithOutputFrom(validate);

        return builder.Build();
    }
}