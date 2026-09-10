using System.ComponentModel;
using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.AI.Tools;

public sealed class RepositoryTools(
    IRepositoryInventory inventory,
    IRepositorySearch search,
    ISourceFileReader sourceFileReader,
    IProjectInspector projectInspector)
{
    [Description(
        "Inspects the selected .NET repository and returns its " +
        "solution files, projects, target frameworks, project " +
        "references and NuGet package references. Call this early " +
        "when answering architectural questions.")]
    public Task<RepositoryOverview> InspectRepositoryAsync(
        CancellationToken cancellationToken)
    {
        return inventory.InspectAsync(
            cancellationToken);
    }

    [Description(
        "Searches allowed text and source files inside the selected " +
        "repository for a literal term. Returns repository-relative " +
        "file paths, exact line numbers and matching lines.")]
    public Task<IReadOnlyList<CodeSearchResult>> SearchCodeAsync(
        [Description(
            "The literal term to search for. Examples: Redis, " +
            "IDistributedCache, AddStackExchangeRedisCache.")]
        string searchTerm,

        [Description(
            "Optional file-name pattern, such as *.cs, *.csproj, " +
            "*.json or *.yml.")]
        string? filePattern,

        CancellationToken cancellationToken)
    {
        return search.SearchAsync(
            searchTerm,
            filePattern,
            cancellationToken);
    }

    [Description(
        "Reads a limited range of lines from one allowed source file " +
        "inside the selected repository. Search first, then use this " +
        "tool to inspect only relevant surrounding lines.")]
    public Task<SourceExcerpt> ReadSourceLinesAsync(
        [Description(
            "Repository-relative source file path.")]
        string relativePath,

        [Description(
            "First line to read, starting at 1.")]
        int firstLine,

        [Description(
            "Last line to read, inclusive.")]
        int lastLine,

        CancellationToken cancellationToken)
    {
        return sourceFileReader.ReadLinesAsync(
            relativePath,
            firstLine,
            lastLine,
            cancellationToken);
    }

    [Description(
        "Parses one .NET .csproj file and returns its target " +
        "framework, project references and package references.")]
    public Task<ProjectDescription> InspectProjectAsync(
        [Description(
            "Repository-relative path to the .csproj file.")]
        string relativeProjectPath,

        CancellationToken cancellationToken)
    {
        return projectInspector.InspectAsync(
            relativeProjectPath,
            cancellationToken);
    }
}