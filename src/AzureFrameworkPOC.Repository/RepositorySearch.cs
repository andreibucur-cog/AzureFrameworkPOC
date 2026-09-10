using System.Text.RegularExpressions;
using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Repository;

public sealed class RepositorySearch(
    IRepositoryContextAccessor contextAccessor,
    RepositoryBoundary boundary,
    RepositoryLimits limits)
    : IRepositorySearch
{
    public async Task<IReadOnlyList<CodeSearchResult>> SearchAsync(
        string searchTerm,
        string? filePattern = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

        if (searchTerm.Length >
            limits.MaximumSearchTermLength)
        {
            throw new ArgumentException(
                $"Search terms cannot exceed " +
                $"{limits.MaximumSearchTermLength} characters.",
                nameof(searchTerm));
        }

        var results =
            new List<CodeSearchResult>();

        foreach (string filePath in EnumerateAllowedFiles(filePattern))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Length >
                limits.MaximumFileSizeBytes)
            {
                continue;
            }

            string[] lines;

            try
            {
                lines = await File.ReadAllLinesAsync(
                    filePath,
                    cancellationToken);
            }
            catch (IOException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

            for (var index = 0;
                 index < lines.Length;
                 index++)
            {
                if (!lines[index].Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                results.Add(
                    new CodeSearchResult(
                        boundary.ToRelativePath(filePath),
                        index + 1,
                        lines[index].Trim()));

                if (results.Count >=
                    limits.MaximumSearchResults)
                {
                    return results;
                }
            }
        }

        return results;
    }

    private IEnumerable<string> EnumerateAllowedFiles(
        string? filePattern)
    {
        string repositoryRoot =
            contextAccessor.Current.RootPath;

        foreach (string filePath in
            Directory.EnumerateFiles(
                repositoryRoot,
                "*",
                SearchOption.AllDirectories))
        {
            string relativePath =
                Path.GetRelativePath(
                    repositoryRoot,
                    filePath);

            string[] segments =
                relativePath.Split(
                    [
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    ],
                    StringSplitOptions.RemoveEmptyEntries);

            if (segments.Any(segment =>
                    limits.IgnoredDirectories.Contains(segment)))
            {
                continue;
            }

            string extension =
                Path.GetExtension(filePath);

            if (!limits.AllowedExtensions.Contains(extension))
            {
                continue;
            }

            if (!MatchesPattern(
                    Path.GetFileName(filePath),
                    filePattern))
            {
                continue;
            }

            yield return filePath;
        }
    }

    private static bool MatchesPattern(
        string fileName,
        string? filePattern)
    {
        if (string.IsNullOrWhiteSpace(filePattern) ||
            filePattern == "*")
        {
            return true;
        }

        string regexPattern =
            "^" +
            Regex.Escape(filePattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") +
            "$";

        return Regex.IsMatch(
            fileName,
            regexPattern,
            RegexOptions.IgnoreCase |
            RegexOptions.CultureInvariant);
    }
}