namespace AzureFrameworkPOC.Core.Exploration;

public sealed class HarnessLimits
{
    public const string SectionName = "Harness";

    public int MaximumOutputTokens { get; init; } = 4_000;

    public int MaximumContextWindowTokens { get; init; } = 24_000;

    public TimeSpan Timeout { get; init; } =
        TimeSpan.FromMinutes(7);

    public bool AllowShellExecution { get; init; } = false;

    public bool AllowRepositoryWrites { get; init; } = false;

    public bool AllowFileDeletion { get; init; } = false;
}