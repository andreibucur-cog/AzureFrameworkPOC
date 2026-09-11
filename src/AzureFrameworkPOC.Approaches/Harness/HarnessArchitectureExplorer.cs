using System.Diagnostics;
using System.Text;
using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;
using Microsoft.Agents.AI;

namespace AzureFrameworkPOC.Approaches.Harness;

public sealed class HarnessArchitectureExplorer(
    ArchitectureHarnessFactory harnessFactory,
    HarnessLimits limits)
    : IArchitectureExplorer
{
    public ExplorerType Type =>
        ExplorerType.Harness;

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

        AIAgent harness =
            await harnessFactory.CreateAsync(
                ChatProviderType.FoundryLocal,
                timeoutSource.Token);

        AgentSession session =
            await harness.CreateSessionAsync(
                timeoutSource.Token);

        string request =
            BuildInvestigationRequest(
                question.Question);

        var responseBuilder =
            new StringBuilder();

        await foreach (AgentResponseUpdate update in
            harness.RunStreamingAsync(
                request,
                session,
                cancellationToken:
                    timeoutSource.Token))
        {
            responseBuilder.Append(update.Text);
        }

        stopwatch.Stop();

        string response =
            responseBuilder.ToString();

        var answer =
            new ArchitectureAnswer(
                Summary: response,
                Claims: [],
                Unknowns: [],
                SuggestedFollowUpQuestions: []);

        return new ExplorerResult(
            Type,
            answer,
            ExplorerMetrics.Empty with
            {
                Duration = stopwatch.Elapsed
            });
    }

    private static string BuildInvestigationRequest(
        string question)
    {
        return $"""
            Perform a comprehensive repository investigation for:

            {question}

            Start by creating an investigation plan and todos.

            At minimum, consider:

            1. Repository and project structure.
            2. Relevant package dependencies.
            3. Registration and configuration.
            4. Interfaces and abstractions.
            5. Concrete implementations.
            6. Consumers and usage paths.
            7. Tests.
            8. Consistency, lifecycle or architectural risks.
            9. Information that cannot be proven from source.

            Use only read-only repository tools.

            Produce a final report containing:

            - executive summary;
            - investigation plan and completed areas;
            - verified findings;
            - evidence for each finding;
            - architectural interpretation;
            - potential risks;
            - uncertainties;
            - recommended follow-up questions.
            """;
    }
}