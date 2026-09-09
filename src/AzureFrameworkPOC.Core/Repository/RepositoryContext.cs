namespace AzureFrameworkPOC.Core.Repository;

public sealed record RepositoryContext
{
    public RepositoryContext(string rootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        var fullPath = Path.GetFullPath(rootPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException(
                $"Repository directory does not exist: {fullPath}");
        }

        RootPath = fullPath;
    }

    public string RootPath { get; }
}