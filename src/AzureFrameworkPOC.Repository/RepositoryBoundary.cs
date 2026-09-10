using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Repository;

public sealed class RepositoryBoundary(
    IRepositoryContextAccessor contextAccessor,
    RepositoryLimits limits)
{
    public string ResolveFile(
        string relativePath,
        bool requireExistingFile = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        RepositoryContext context =
            contextAccessor.Current;

        string repositoryRoot =
            EnsureTrailingSeparator(
                Path.GetFullPath(context.RootPath));

        string candidate =
            Path.GetFullPath(
                Path.Combine(
                    repositoryRoot,
                    relativePath));

        if (!candidate.StartsWith(
                repositoryRoot,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "The requested path is outside the selected repository.");
        }

        EnsurePathIsAllowed(candidate, repositoryRoot);

        if (requireExistingFile &&
            !File.Exists(candidate))
        {
            throw new FileNotFoundException(
                "The requested repository file does not exist.",
                relativePath);
        }

        return candidate;
    }

    public string ToRelativePath(string absolutePath)
    {
        return Path.GetRelativePath(
                contextAccessor.Current.RootPath,
                absolutePath)
            .Replace('\\', '/');
    }

    private void EnsurePathIsAllowed(
        string candidate,
        string repositoryRoot)
    {
        string relativePath =
            Path.GetRelativePath(
                repositoryRoot,
                candidate);

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
            throw new UnauthorizedAccessException(
                "The requested path belongs to an ignored directory.");
        }

        string extension =
            Path.GetExtension(candidate);

        if (!limits.AllowedExtensions.Contains(extension))
        {
            throw new UnauthorizedAccessException(
                $"Files with extension '{extension}' are not allowed.");
        }
    }

    private static string EnsureTrailingSeparator(
        string path)
    {
        return path.EndsWith(
            Path.DirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
    }
}