namespace AzureFrameworkPOC.Core.Repository;

public interface IRepositoryInventory
{
    Task<RepositoryOverview> InspectAsync(
        CancellationToken cancellationToken = default);
}