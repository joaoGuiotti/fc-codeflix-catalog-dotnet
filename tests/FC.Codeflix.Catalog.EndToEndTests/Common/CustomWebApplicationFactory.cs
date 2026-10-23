using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("e2e");
        builder.ConfigureServices(services =>
        {
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
