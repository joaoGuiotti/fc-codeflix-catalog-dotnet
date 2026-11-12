using FC.Codeflix.Catalog.Infra.Data.EF;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("e2e");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.First(s =>
                s.ServiceType == typeof(StorageClient));
            services.Remove(descriptor);
            services.AddScoped(sp => Mock.Of<StorageClient>());

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider
                    .GetService<CodeflixCatalogDbContext>();
                ArgumentNullException.ThrowIfNull(ctx);
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        });

        base.ConfigureWebHost(builder);
    }
}
