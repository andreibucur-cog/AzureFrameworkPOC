namespace AzureFrameworkPOC.Core.Repository;

public interface IRepositorySearch
{
    Task<IReadOnlyList<CodeSearchResult>> SearchAsync(
        string searchTerm,
        string? filePattern = null,
        CancellationToken cancellationToken = default);
}