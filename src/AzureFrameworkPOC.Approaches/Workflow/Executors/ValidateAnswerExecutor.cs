using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow.Executors;

public sealed class ValidateAnswerExecutor()
    : Executor<
        DraftArchitectureAnswer,
        ValidatedArchitectureAnswer>(
        nameof(ValidateAnswerExecutor))
{
    public override ValueTask<ValidatedArchitectureAnswer>
        HandleAsync(
            DraftArchitectureAnswer message,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
    {
        var warnings =
            new List<string>();

        if (string.IsNullOrWhiteSpace(message.Content))
        {
            warnings.Add(
                "The synthesis agent returned an empty response.");
        }

        if (message.AvailableEvidence.Count == 0)
        {
            warnings.Add(
                "No source evidence was available for verification.");
        }

        IReadOnlyList<string> availablePaths =
            message.AvailableEvidence
                .Select(item =>
                    item.Excerpt.RelativePath)
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        int referencedPaths =
            availablePaths.Count(path =>
                message.Content.Contains(
                    path,
                    StringComparison.OrdinalIgnoreCase));

        if (availablePaths.Count > 0 &&
            referencedPaths == 0)
        {
            warnings.Add(
                "The generated answer did not reference any " +
                "available evidence paths.");
        }

        return ValueTask.FromResult(
            new ValidatedArchitectureAnswer(
                message.Content,
                message.AvailableEvidence.Count,
                warnings));
    }
}