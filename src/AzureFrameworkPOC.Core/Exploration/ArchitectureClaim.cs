namespace AzureFrameworkPOC.Core.Exploration;

public sealed record ArchitectureClaim(
    string Claim,
    string Confidence,
    IReadOnlyList<CodeEvidence> Evidence);