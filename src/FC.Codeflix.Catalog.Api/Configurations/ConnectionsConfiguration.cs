using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{

    public static IServiceCollection AddApiConnections(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext(configuration);
        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        services.AddDbContext<CodeflixCatalogDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            ));
        return services;
    }
}
