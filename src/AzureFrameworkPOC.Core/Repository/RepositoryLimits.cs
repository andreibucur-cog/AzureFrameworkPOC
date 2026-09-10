namespace AzureFrameworkPOC.Core.Repository;

public sealed class RepositoryLimits
{
    public const string SectionName = "Repository";

    public int MaximumFilesPerListing { get; init; } = 100;

    public int MaximumSearchResults { get; init; } = 50;

    public int MaximumLinesPerRead { get; init; } = 150;

    public int MaximumSearchTermLength { get; init; } = 200;

    public long MaximumFileSizeBytes { get; init; } =
        1 * 1024 * 1024;

    public IReadOnlySet<string> AllowedExtensions { get; init; } =
        new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ".cs",
            ".csproj",
            ".sln",
            ".slnx",
            ".json",
            ".xml",
            ".config",
            ".props",
            ".targets",
            ".md",
            ".yml",
            ".yaml",
            ".razor",
            ".cshtml",
            ".ts",
            ".tsx",
            ".js",
            ".jsx"
        };

    public IReadOnlySet<string> IgnoredDirectories { get; init; } =
        new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ".git",
            ".vs",
            ".idea",
            "bin",
            "obj",
            "node_modules",
            "packages",
            "TestResults",
            "coverage"
        };
}