using Bogus;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Common;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture() => Faker = new Faker("pt_BR");

    public IServiceCollection serviceCollection = new ServiceCollection();

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("integration-tests-db")
                .Options
        );
        if (preserveData == false)
            context.Database.EnsureDeleted();
        return context;
    }

    public bool GetRandomBoolean()
        => Faker.Random.Bool();

    public IUnitOfWork GetUnitOfWork(CodeflixCatalogDbContext? dbContext = null)
    {
        
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var publisher = new DomainEventPublisher(serviceProvider);
        var logger = serviceProvider.GetService<ILogger<UnitOfWork>>();
        return new UnitOfWork(dbContext ?? CreateDbContext(), publisher, logger!);
    }
}
