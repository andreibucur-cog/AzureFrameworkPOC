using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace AzureFrameworkPOC.Approaches.Workflow.Executors;

public sealed class SynthesizeAnswerExecutor(
    AIAgent synthesisAgent)
    : Executor<CollectedEvidence, DraftArchitectureAnswer>(
        nameof(SynthesizeAnswerExecutor))
{
    public override async ValueTask<DraftArchitectureAnswer>
        HandleAsync(
            CollectedEvidence message,
            IWorkflowContext context,
            CancellationToken cancellationToken = default)
    {
        string prompt =
            BuildPrompt(message);

        AgentResponse response =
            await synthesisAgent.RunAsync(
                prompt,
                cancellationToken: cancellationToken);

        return new DraftArchitectureAnswer(
            message.Question,
            response.ToString(),
            message.Evidence);
    }

    private static string BuildPrompt(
        CollectedEvidence message)
    {
        var builder =
            new StringBuilder();

        builder.AppendLine(
            "Architecture question:");

        builder.AppendLine(message.Question);
        builder.AppendLine();

        builder.AppendLine(
            "Repository evidence:");

        if (message.Evidence.Count == 0)
        {
            builder.AppendLine(
                "No matching repository evidence was found.");
        }

        foreach (EvidenceItem item in message.Evidence)
        {
            builder.AppendLine();
            builder.AppendLine(
                $"Search term: {item.SearchTerm}");

            builder.AppendLine(
                $"Search purpose: {item.Purpose}");

            builder.AppendLine(
                $"File: {item.Excerpt.RelativePath}");

            builder.AppendLine(
                $"Lines: {item.Excerpt.FirstLine}-" +
                $"{item.Excerpt.LastLine}");

            builder.AppendLine("Source:");
            builder.AppendLine(item.Excerpt.Content);
            builder.AppendLine("---");
        }

        return builder.ToString();
    }
}