namespace AzureFrameworkPOC.Core.Repository;

public sealed record RepositoryOverview(
    IReadOnlyList<string> SolutionFiles,
    IReadOnlyList<string> ProjectFiles,
    IReadOnlyList<ProjectDescription> Projects);