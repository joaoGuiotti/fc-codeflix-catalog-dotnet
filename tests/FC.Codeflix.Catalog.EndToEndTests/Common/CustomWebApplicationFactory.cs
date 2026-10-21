using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbOptions = services.FirstOrDefault(
                x => x.ServiceType == typeof(DbContextOptions<CodeflixCatalogDbContext>)
            );
            if (dbOptions != null)
                services.Remove(dbOptions);
            services.AddDbContext<CodeflixCatalogDbContext>(options => {
                options.UseInMemoryDatabase("e2e-tests-db");
            });
        });

        base.ConfigureWebHost(builder);
    }
}
