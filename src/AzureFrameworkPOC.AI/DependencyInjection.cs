using AzureFrameworkPOC.AI.Providers;
using AzureFrameworkPOC.AI.Providers.FoundryLocal;
using AzureFrameworkPOC.Core.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AzureFrameworkPOC.AI.Tools;

namespace AzureFrameworkPOC.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<FoundryLocalOptions>()
            .Bind(configuration.GetSection(
                FoundryLocalOptions.SectionName))
            .Validate(
                options => Uri.TryCreate(
                    options.Endpoint,
                    UriKind.Absolute,
                    out _),
                "Foundry Local endpoint must be an absolute URI.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(
                        options.ModelAlias),
                "Foundry Local model alias is required.")
            .ValidateOnStart();

        services.AddSingleton<RepositoryTools>();
        services.AddSingleton<ArchitectureToolFactory>();

        services.AddSingleton<
            IFoundryLocalRuntime,
            FoundryLocalRuntime>();

        services.AddSingleton<
            IChatClientProvider,
            FoundryLocalChatClientProvider>();

        services.AddSingleton<
            IChatClientProviderResolver,
            ChatClientProviderResolver>();

        return services;
    }
}