using System.Xml.Linq;
using AzureFrameworkPOC.Core.Repository;

namespace AzureFrameworkPOC.Repository;

public sealed class ProjectInspector(
    RepositoryBoundary boundary)
    : IProjectInspector
{
    public async Task<ProjectDescription> InspectAsync(
        string relativeProjectPath,
        CancellationToken cancellationToken = default)
    {
        if (!relativeProjectPath.EndsWith(
                ".csproj",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "The path must refer to a .csproj file.",
                nameof(relativeProjectPath));
        }

        string fullPath =
            boundary.ResolveFile(relativeProjectPath);

        await using FileStream stream =
            File.OpenRead(fullPath);

        XDocument document =
            await XDocument.LoadAsync(
                stream,
                LoadOptions.None,
                cancellationToken);

        string? targetFramework =
            document
                .Descendants("TargetFramework")
                .Select(element => element.Value)
                .FirstOrDefault();

        targetFramework ??=
            document
                .Descendants("TargetFrameworks")
                .Select(element => element.Value)
                .FirstOrDefault();

        IReadOnlyList<string> projectReferences =
            document
                .Descendants("ProjectReference")
                .Select(element =>
                    element.Attribute("Include")?.Value)
                .Where(value =>
                    !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .ToList();

        IReadOnlyList<PackageReferenceInfo> packageReferences =
            document
                .Descendants("PackageReference")
                .Select(element =>
                    new PackageReferenceInfo(
                        element.Attribute("Include")?.Value
                            ?? element.Attribute("Update")?.Value
                            ?? "Unknown",

                        element.Attribute("Version")?.Value
                            ?? element
                                .Element("Version")
                                ?.Value))
                .ToList();

        return new ProjectDescription(
            boundary.ToRelativePath(fullPath),
            targetFramework,
            projectReferences,
            packageReferences);
    }
}