namespace AzureFrameworkPOC.Core.Repository;

public sealed record RepositoryFileInfo(
    string RelativePath,
    string Extension,
    long SizeBytes);