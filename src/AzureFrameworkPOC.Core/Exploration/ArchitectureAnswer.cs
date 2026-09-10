namespace AzureFrameworkPOC.Core.Exploration;

public sealed record ArchitectureAnswer(
    string Summary,
    IReadOnlyList<ArchitectureClaim> Claims,
    IReadOnlyList<string> Unknowns,
    IReadOnlyList<string> SuggestedFollowUpQuestions);