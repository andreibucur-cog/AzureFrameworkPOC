namespace AzureFrameworkPOC.Core.Exploration;

public sealed record ExplorerResult(
    ExplorerType Type,
    ArchitectureAnswer Answer,
    ExplorerMetrics Metrics);