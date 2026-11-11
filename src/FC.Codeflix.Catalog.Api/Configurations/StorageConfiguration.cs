using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using FC.Codeflix.Catalog.Infra.Storage.Service;
using Google.Cloud.Storage.V1;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StorageServiceOptions>(
            configuration.GetSection(nameof(StorageServiceOptions))
        );
        services.AddSingleton(_ => StorageClient.Create());
        services.AddTransient<IStorageService, StorageService>();
        return services;
    }
}

