namespace AzureFrameworkPOC.Core.Repository;

public interface ISourceFileReader
{
    Task<SourceExcerpt> ReadLinesAsync(
        string relativePath,
        int firstLine,
        int lastLine,
        CancellationToken cancellationToken = default);
}