using AzureFrameworkPOC.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AzureFrameworkPOC.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryExploration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RepositoryLimits>()
            .Bind(configuration.GetSection(
                RepositoryLimits.SectionName))
            .ValidateOnStart();

        services.AddSingleton(sp =>
            sp.GetRequiredService<
                IOptions<RepositoryLimits>>()
                .Value);

        services.AddSingleton<
            IRepositoryContextAccessor,
            RepositoryContextAccessor>();

        services.AddSingleton<RepositoryBoundary>();

        services.AddSingleton<
            IRepositoryInventory,
            RepositoryInventory>();

        services.AddSingleton<
            IRepositorySearch,
            RepositorySearch>();

        services.AddSingleton<
            ISourceFileReader,
            SourceFileReader>();

        services.AddSingleton<
            IProjectInspector,
            ProjectInspector>();

        return services;
    }
}