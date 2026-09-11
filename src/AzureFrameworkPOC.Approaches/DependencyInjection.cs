using AzureFrameworkPOC.Approaches.Agent;
using AzureFrameworkPOC.Approaches.Harness;
using AzureFrameworkPOC.Approaches.Workflow;
using AzureFrameworkPOC.Core.Exploration;
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

        services
            .AddOptions<HarnessLimits>()
            .Bind(configuration.GetSection(
                HarnessLimits.SectionName))
            .Validate(
                options =>
                    options.MaximumOutputTokens > 0,
                "Harness output-token limit must be positive.")
            .Validate(
                options =>
                    !options.AllowShellExecution,
                "Shell execution must remain disabled for the initial POC.")
            .ValidateOnStart();

        services.AddSingleton(sp =>
            sp.GetRequiredService<
                    Microsoft.Extensions.Options
                        .IOptions<HarnessLimits>>()
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

        // Harness
        services.AddSingleton<
            ArchitectureHarnessFactory>();

        services.AddTransient<
            IArchitectureExplorer,
            HarnessArchitectureExplorer>();

        return services;
    }
}