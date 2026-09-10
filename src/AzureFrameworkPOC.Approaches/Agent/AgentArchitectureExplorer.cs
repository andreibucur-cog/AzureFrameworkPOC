using System.Diagnostics;
using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;
using Microsoft.Agents.AI;

namespace AzureFrameworkPOC.Approaches.Agent;

public sealed class AgentArchitectureExplorer(
    ArchitectureAgentFactory agentFactory)
    : IArchitectureExplorer
{
    public ExplorerType Type =>
        ExplorerType.Agent;

    public async Task<ExplorerResult> ExploreAsync(
        ArchitectureQuestion question,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            question.Question);

        var stopwatch =
            Stopwatch.StartNew();

        AIAgent agent =
            await agentFactory.CreateAsync(
                ChatProviderType.FoundryLocal,
                cancellationToken);

        AgentSession session =
            await agent.CreateSessionAsync(
                cancellationToken);

        string request =
            BuildRequest(question.Question);

        AgentResponse response =
            await agent.RunAsync(
                request,
                session,
                cancellationToken: cancellationToken);

        stopwatch.Stop();

        var answer =
            new ArchitectureAnswer(
                Summary: response.ToString(),
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

    private static string BuildRequest(
        string question)
    {
        return $"""
            Investigate the selected repository and answer:

            {question}

            Use repository tools to gather evidence before answering.

            Organize the answer as:

            1. Summary
            2. Verified findings
            3. Evidence for each finding
            4. Uncertainties
            5. Suggested follow-up questions
            """;
    }
}