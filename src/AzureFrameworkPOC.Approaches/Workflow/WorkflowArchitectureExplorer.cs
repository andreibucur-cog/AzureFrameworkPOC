using System.Diagnostics;
using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;
using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed class WorkflowArchitectureExplorer(
    ArchitectureWorkflowFactory workflowFactory,
    WorkflowLimits limits)
    : IArchitectureExplorer
{
    public ExplorerType Type =>
        ExplorerType.Workflow;

    public async Task<ExplorerResult> ExploreAsync(
        ArchitectureQuestion question,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            question.Question);

        using var timeoutSource =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        timeoutSource.CancelAfter(limits.Timeout);

        var stopwatch =
            Stopwatch.StartNew();

        var workflow =
            await workflowFactory.CreateAsync(
                ChatProviderType.FoundryLocal,
                timeoutSource.Token);

        var input =
            new WorkflowRequest(
                question.Question, null);

        await using Run run =
            await InProcessExecution.RunAsync(
                workflow,
                input,
                cancellationToken: timeoutSource.Token);

        ValidatedArchitectureAnswer? output = null;

        foreach (WorkflowEvent workflowEvent in
                 run.NewEvents)
        {
            if (workflowEvent is
                WorkflowOutputEvent outputEvent &&
                outputEvent.Data is
                    ValidatedArchitectureAnswer validated)
            {
                output = validated;
            }
        }

        stopwatch.Stop();

        if (output is null)
        {
            throw new InvalidOperationException(
                "The workflow completed without a valid output.");
        }

        var answer =
            new ArchitectureAnswer(
                Summary: output.Content,
                Claims: [],
                Unknowns:
                    output.ValidationWarnings,
                SuggestedFollowUpQuestions: []);

        return new ExplorerResult(
            Type,
            answer,
            ExplorerMetrics.Empty with
            {
                Duration = stopwatch.Elapsed
            });
    }
}