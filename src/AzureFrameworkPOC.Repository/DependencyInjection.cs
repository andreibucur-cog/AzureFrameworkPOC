using AzureFrameworkPOC.Core.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFrameworkPOC.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryExploration(
        this IServiceCollection services)
    {
        services.AddSingleton<
            IRepositoryContextAccessor,
            RepositoryContextAccessor>();

        return services;
    }
}