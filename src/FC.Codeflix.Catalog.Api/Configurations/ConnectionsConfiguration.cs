using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{

    public static IServiceCollection AddApiConnections(this IServiceCollection services)
    {
        services.AddDbContext();
        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddDbContext<CodeflixCatalogDbContext>(options =>
            options.UseInMemoryDatabase("InMemory-Database"));
        return services;
    }
}
