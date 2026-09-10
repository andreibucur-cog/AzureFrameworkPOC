using AzureFrameworkPOC.Approaches.Agent;
using AzureFrameworkPOC.Core.Exploration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFrameworkPOC.Approaches;

public static class DependencyInjection
{
    public static IServiceCollection AddApproaches(
        this IServiceCollection services)
    {
        services.AddSingleton<
            ArchitectureAgentFactory>();

        services.AddTransient<
            IArchitectureExplorer,
            AgentArchitectureExplorer>();

        return services;
    }
}