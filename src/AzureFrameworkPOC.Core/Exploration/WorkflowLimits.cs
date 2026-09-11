namespace AzureFrameworkPOC.Core.Exploration;

public sealed class WorkflowLimits
{
    public const string SectionName = "Workflow";

    public int MaximumSearchOperations { get; init; } = 8;

    public int MaximumMatchesPerOperation { get; init; } = 15;

    public int MaximumFilesToRead { get; init; } = 12;

    public int ContextLinesBeforeMatch { get; init; } = 8;

    public int ContextLinesAfterMatch { get; init; } = 16;

    public int MaximumEvidenceItems { get; init; } = 16;

    public int MaximumOutputTokens { get; init; } = 2_500;

    public int MaximumRepairAttempts { get; init; } = 1;

    public TimeSpan Timeout { get; init; } =
        TimeSpan.FromMinutes(4);
}