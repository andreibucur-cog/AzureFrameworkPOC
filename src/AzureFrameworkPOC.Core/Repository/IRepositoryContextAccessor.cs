namespace AzureFrameworkPOC.Core.Repository;

public interface IRepositoryContextAccessor
{
    RepositoryContext Current { get; }

    void SetRepository(string rootPath);
}