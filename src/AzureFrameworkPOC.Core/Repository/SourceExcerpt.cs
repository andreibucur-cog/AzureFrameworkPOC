namespace AzureFrameworkPOC.Core.Repository;

public sealed record SourceExcerpt(
    string RelativePath,
    int FirstLine,
    int LastLine,
    string Content);