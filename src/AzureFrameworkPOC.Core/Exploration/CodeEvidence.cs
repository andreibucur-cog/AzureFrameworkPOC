namespace AzureFrameworkPOC.Core.Exploration;

public sealed record CodeEvidence(
    string RelativePath,
    int FirstLine,
    int LastLine,
    string Excerpt,
    string Explanation);