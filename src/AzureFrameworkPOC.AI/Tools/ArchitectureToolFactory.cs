using Microsoft.Extensions.AI;

namespace AzureFrameworkPOC.AI.Tools;

public sealed class ArchitectureToolFactory(
    RepositoryTools repositoryTools)
{
    private IReadOnlyList<AITool>? _cachedTools;

    public IReadOnlyList<AITool> CreateReadOnlyTools()
    {
        return _cachedTools ??=
        [
            AIFunctionFactory.Create(
                repositoryTools.InspectRepositoryAsync),

            AIFunctionFactory.Create(
                repositoryTools.SearchCodeAsync),

            AIFunctionFactory.Create(
                repositoryTools.ReadSourceLinesAsync),

            AIFunctionFactory.Create(
                repositoryTools.InspectProjectAsync)
        ];
    }
}