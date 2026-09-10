namespace AzureFrameworkPOC.Core.Repository;

public sealed record PackageReferenceInfo(
    string Name,
    string? Version);