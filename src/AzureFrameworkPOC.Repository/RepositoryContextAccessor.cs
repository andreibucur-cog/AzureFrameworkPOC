using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Repository;

public sealed class RepositoryContextAccessor
    : IRepositoryContextAccessor
{
    private RepositoryContext? _current;

    public RepositoryContext Current =>
        _current
        ?? throw new InvalidOperationException(
            "No repository has been selected.");

    public void SetRepository(string rootPath)
    {
        _current = new RepositoryContext(rootPath);
    }
}