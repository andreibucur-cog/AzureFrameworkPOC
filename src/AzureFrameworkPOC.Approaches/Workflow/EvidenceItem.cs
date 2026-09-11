using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed record EvidenceItem(
    string SearchTerm,
    string Purpose,
    SourceExcerpt Excerpt);