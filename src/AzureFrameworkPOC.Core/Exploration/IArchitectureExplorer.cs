namespace AzureFrameworkPOC.Core.Exploration;

public interface IArchitectureExplorer
{
    ExplorerType Type { get; }

    Task<ExplorerResult> ExploreAsync(
        ArchitectureQuestion question,
        CancellationToken cancellationToken = default);
}