using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Repository;

public sealed class RepositoryInventory(
    IRepositoryContextAccessor contextAccessor,
    IProjectInspector projectInspector,
    RepositoryLimits limits)
    : IRepositoryInventory
{
    public async Task<RepositoryOverview> InspectAsync(
        CancellationToken cancellationToken = default)
    {
        string root =
            contextAccessor.Current.RootPath;

        IReadOnlyList<string> solutionFiles =
            Directory
                .EnumerateFiles(
                    root,
                    "*.*",
                    SearchOption.AllDirectories)
                .Where(path =>
                    path.EndsWith(
                        ".sln",
                        StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(
                        ".slnx",
                        StringComparison.OrdinalIgnoreCase))
                .Where(path => !IsIgnored(root, path))
                .Take(limits.MaximumFilesPerListing)
                .Select(path =>
                    Path.GetRelativePath(root, path)
                        .Replace('\\', '/'))
                .ToList();

        IReadOnlyList<string> projectFiles =
            Directory
                .EnumerateFiles(
                    root,
                    "*.csproj",
                    SearchOption.AllDirectories)
                .Where(path => !IsIgnored(root, path))
                .Take(limits.MaximumFilesPerListing)
                .Select(path =>
                    Path.GetRelativePath(root, path)
                        .Replace('\\', '/'))
                .ToList();

        var projects =
            new List<ProjectDescription>();

        foreach (string projectFile in projectFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            projects.Add(
                await projectInspector.InspectAsync(
                    projectFile,
                    cancellationToken));
        }

        return new RepositoryOverview(
            solutionFiles,
            projectFiles,
            projects);
    }

    private bool IsIgnored(
        string root,
        string path)
    {
        string relativePath =
            Path.GetRelativePath(root, path);

        string[] segments =
            relativePath.Split(
                [
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                ],
                StringSplitOptions.RemoveEmptyEntries);

        return segments.Any(segment =>
            limits.IgnoredDirectories.Contains(segment));
    }
}