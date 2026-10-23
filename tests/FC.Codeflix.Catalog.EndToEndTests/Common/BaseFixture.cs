using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public CustomWebApplicationFactory<Program> webAppFactory { get; set; }
    public HttpClient httpClient { get; set; }
    public ApiClient ApiClient { get; set; }
    private readonly string _dbConnectionStirng;

    public BaseFixture()
    {
        Faker = new Faker();
        webAppFactory = new CustomWebApplicationFactory<Program>();
        httpClient = webAppFactory.CreateClient();
        ApiClient = new ApiClient(httpClient);
        var configuration = webAppFactory.Services.GetService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionStirng = ((IConfiguration)configuration).GetConnectionString("CatalogDb")!;
    }
    public CodeflixCatalogDbContext CreateDbContext()
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseMySql(
                _dbConnectionStirng,
                ServerVersion.AutoDetect(_dbConnectionStirng)
            ).Options
        );
        return context;
    }

    public void DisposePersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}

