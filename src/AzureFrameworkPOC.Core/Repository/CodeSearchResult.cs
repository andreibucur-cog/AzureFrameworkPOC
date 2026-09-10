namespace AzureFrameworkPOC.Core.Repository;

public sealed record CodeSearchResult(
    string RelativePath,
    int LineNumber,
    string MatchingLine);