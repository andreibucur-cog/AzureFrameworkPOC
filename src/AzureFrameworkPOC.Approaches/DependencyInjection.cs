using AzureFrameworkPOC.Approaches.Agent;
using AzureFrameworkPOC.Core.Exploration;
using AzureFrameworkPOC.Approaches.Workflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFrameworkPOC.Approaches;

public static class DependencyInjection
{
    public static IServiceCollection AddApproaches(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services
            .AddOptions<WorkflowLimits>()
            .Bind(configuration.GetSection(
                WorkflowLimits.SectionName))
            .Validate(
                options =>
                    options.MaximumSearchOperations > 0,
                "Workflow must allow at least one search operation.")
            .Validate(
                options =>
                    options.MaximumEvidenceItems > 0,
                "Workflow must allow at least one evidence item.")
            .ValidateOnStart();

        services.AddSingleton(sp =>
            sp.GetRequiredService<
                    Microsoft.Extensions.Options
                        .IOptions<WorkflowLimits>>()
                .Value);

        // Agent
        services.AddSingleton<
            ArchitectureAgentFactory>();

        services.AddTransient<
            IArchitectureExplorer,
            AgentArchitectureExplorer>();

        // Workflow
        services.AddSingleton<
            SearchPlanningAgentFactory>();

        services.AddSingleton<
            SynthesisAgentFactory>();

        services.AddSingleton<
            ArchitectureWorkflowFactory>();

        services.AddTransient<
            IArchitectureExplorer,
            WorkflowArchitectureExplorer>();

        return services;
    }
}