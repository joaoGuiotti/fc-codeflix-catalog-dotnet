using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Common;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture() => Faker = new Faker("pt_BR");

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(CreateDbContextOptions());
        if (!preserveData)
        {
            dbContext.Database.EnsureCreated();
        }
        return dbContext;
    }

    private DbContextOptions<CodeflixCatalogDbContext> CreateDbContextOptions(
        bool preserveData = false
    )
    {
        var dbName = preserveData
            ? "IntegrationTestsDb-UnitOfWork"
            : $"IntegrationTestsDb-UnitOfWork_{Guid.NewGuid()}";
        return new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }
}
