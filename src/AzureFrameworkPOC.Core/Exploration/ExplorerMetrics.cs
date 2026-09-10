namespace AzureFrameworkPOC.Core.Exploration;

public sealed record ExplorerMetrics(
    TimeSpan Duration,
    int ModelCalls,
    int ToolCalls,
    int FilesInspected,
    int ClaimsProduced,
    int ClaimsVerified,
    int ClaimsRejected)
{
    public static ExplorerMetrics Empty =>
        new(
            TimeSpan.Zero,
            ModelCalls: 0,
            ToolCalls: 0,
            FilesInspected: 0,
            ClaimsProduced: 0,
            ClaimsVerified: 0,
            ClaimsRejected: 0);
}