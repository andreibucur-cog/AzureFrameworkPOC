namespace AzureFrameworkPOC.Core.Repository;

public sealed record ProjectDescription(
    string RelativePath,
    string? TargetFramework,
    IReadOnlyList<string> ProjectReferences,
    IReadOnlyList<PackageReferenceInfo> PackageReferences);