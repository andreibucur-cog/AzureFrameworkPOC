namespace AzureFrameworkPOC.Core.Repository;

public interface IProjectInspector
{
    Task<ProjectDescription> InspectAsync(
        string relativeProjectPath,
        CancellationToken cancellationToken = default);
}