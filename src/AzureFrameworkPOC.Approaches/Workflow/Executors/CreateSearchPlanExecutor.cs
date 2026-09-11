using AzureFrameworkPOC.Approaches.Workflow;
using AzureFrameworkPOC.Core.Exploration;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureFrameworkPOC.Approaches.Workflow.Executors;

public sealed class CreateSearchPlanExecutor(
    AIAgent planningAgent,
    WorkflowLimits limits)
    : Executor<WorkflowRequest, SearchPlan>(
        nameof(CreateSearchPlanExecutor))
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling =
                JsonUnmappedMemberHandling.Skip
        };

    public override async ValueTask<SearchPlan> HandleAsync(
        WorkflowRequest message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        RepositoryOverview overview =
            message.RepositoryOverview
            ?? throw new InvalidOperationException(
                "Repository overview was not produced.");

        string prompt =
            BuildPrompt(
                message.Question,
                overview);

        AgentResponse response =
            await planningAgent.RunAsync(
                prompt,
                cancellationToken: cancellationToken);

        SearchPlanResponse? planResponse =
            DeserializePlan(response.ToString());

        IReadOnlyList<SearchOperation> operations =
            NormalizeOperations(planResponse);

        return new SearchPlan(
            message.Question,
            overview,
            operations);
    }

    private IReadOnlyList<SearchOperation> NormalizeOperations(
        SearchPlanResponse? response)
    {
        IEnumerable<SearchPlanOperation> source =
            response?.Operations
            ?? [];

        return source
            .Where(operation =>
                !string.IsNullOrWhiteSpace(
                    operation.SearchTerm))
            .Select(operation =>
                new SearchOperation(
                    operation.SearchTerm.Trim(),
                    NormalizePattern(operation.FilePattern),
                    operation.Purpose?.Trim()
                        ?? "Relevant repository search"))
            .DistinctBy(operation =>
                (
                    operation.SearchTerm.ToUpperInvariant(),
                    operation.FilePattern?.ToUpperInvariant()
                ))
            .Take(limits.MaximumSearchOperations)
            .ToList();
    }

    private static string? NormalizePattern(
        string? pattern)
    {
        return string.IsNullOrWhiteSpace(pattern)
            ? null
            : pattern.Trim();
    }

    private static SearchPlanResponse? DeserializePlan(
        string response)
    {
        string cleaned =
            response
                .Replace("```json", string.Empty)
                .Replace("```", string.Empty)
                .Trim();

        try
        {
            return JsonSerializer.Deserialize<SearchPlanResponse>(
                cleaned,
                JsonOptions);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "The planning agent did not return valid JSON.",
                exception);
        }
    }

    private static string BuildPrompt(
        string question,
        RepositoryOverview overview)
    {
        string projects =
            string.Join(
                Environment.NewLine,
                overview.Projects.Select(project =>
                    $"""
                    Project: {project.RelativePath}
                    Framework: {project.TargetFramework ?? "Unknown"}
                    Packages: {string.Join(
                        ", ",
                        project.PackageReferences.Select(
                            package =>
                                package.Version is null
                                    ? package.Name
                                    : $"{package.Name} {package.Version}"))}
                    References: {string.Join(
                        ", ",
                        project.ProjectReferences)}
                    """));

        return $"""
            Architecture question:

            {question}

            Repository projects:

            {projects}

            Create a precise source-code search plan.
            """;
    }

    private sealed record SearchPlanResponse(
        IReadOnlyList<SearchPlanOperation>? Operations);

    private sealed record SearchPlanOperation(
        string SearchTerm,
        string? FilePattern,
        string? Purpose);
}