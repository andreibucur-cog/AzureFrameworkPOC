using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Repository;

public sealed class SourceFileReader(
    RepositoryBoundary boundary,
    RepositoryLimits limits)
    : ISourceFileReader
{
    public async Task<SourceExcerpt> ReadLinesAsync(
        string relativePath,
        int firstLine,
        int lastLine,
        CancellationToken cancellationToken = default)
    {
        if (firstLine < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstLine),
                "The first line must be at least 1.");
        }

        if (lastLine < firstLine)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lastLine),
                "The last line must not precede the first line.");
        }

        int requestedLineCount =
            lastLine - firstLine + 1;

        if (requestedLineCount >
            limits.MaximumLinesPerRead)
        {
            throw new InvalidOperationException(
                $"A maximum of " +
                $"{limits.MaximumLinesPerRead} lines " +
                "can be read in one operation.");
        }

        string fullPath =
            boundary.ResolveFile(relativePath);

        var fileInfo =
            new FileInfo(fullPath);

        if (fileInfo.Length >
            limits.MaximumFileSizeBytes)
        {
            throw new InvalidOperationException(
                $"The file exceeds the maximum size of " +
                $"{limits.MaximumFileSizeBytes} bytes.");
        }

        string[] lines =
            await File.ReadAllLinesAsync(
                fullPath,
                cancellationToken);

        if (firstLine > lines.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstLine),
                $"The file contains only {lines.Length} lines.");
        }

        int effectiveLastLine =
            Math.Min(lastLine, lines.Length);

        string content =
            string.Join(
                Environment.NewLine,
                lines[
                    (firstLine - 1)..effectiveLastLine]);

        return new SourceExcerpt(
            boundary.ToRelativePath(fullPath),
            firstLine,
            effectiveLastLine,
            content);
    }
}