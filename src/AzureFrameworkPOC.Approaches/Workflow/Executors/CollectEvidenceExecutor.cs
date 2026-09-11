using AzureFrameworkPOC.Approaches.Workflow;
using AzureFrameworkPOC.Core.Exploration;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow.Executors;

public sealed class CollectEvidenceExecutor(
    ISourceFileReader sourceFileReader,
    WorkflowLimits limits)
    : Executor<SearchExecutionResult, CollectedEvidence>(
        nameof(CollectEvidenceExecutor))
{
    public override async ValueTask<CollectedEvidence> HandleAsync(
        SearchExecutionResult message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var evidence =
            new List<EvidenceItem>();

        var candidates =
            message.Searches
                .SelectMany(search =>
                    search.Matches.Select(match =>
                        new
                        {
                            search.Operation.SearchTerm,
                            search.Operation.Purpose,
                            Match = match
                        }))
                .GroupBy(candidate =>
                    (
                        candidate.Match.RelativePath,
                        candidate.Match.LineNumber
                    ))
                .Select(group => group.First())
                .Take(limits.MaximumFilesToRead)
                .ToList();

        foreach (var candidate in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int firstLine =
                Math.Max(
                    1,
                    candidate.Match.LineNumber -
                    limits.ContextLinesBeforeMatch);

            int lastLine =
                candidate.Match.LineNumber +
                limits.ContextLinesAfterMatch;

            try
            {
                SourceExcerpt excerpt =
                    await sourceFileReader.ReadLinesAsync(
                        candidate.Match.RelativePath,
                        firstLine,
                        lastLine,
                        cancellationToken);

                evidence.Add(
                    new EvidenceItem(
                        candidate.SearchTerm,
                        candidate.Purpose,
                        excerpt));
            }
            catch (Exception exception)
                when (exception is
                    IOException or
                    UnauthorizedAccessException or
                    ArgumentException or
                    InvalidOperationException)
            {
                // Continue processing other valid evidence.
                // Add structured logging here later.
            }

            if (evidence.Count >=
                limits.MaximumEvidenceItems)
            {
                break;
            }
        }

        return new CollectedEvidence(
            message.Question,
            message.RepositoryOverview,
            evidence);
    }
}