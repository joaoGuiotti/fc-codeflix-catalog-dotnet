using FC.Codeflix.Catalog.Domain.SeedWork;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixtureCollection))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetExampleCategoryList();
        var categoryWithEvents = exampleCategoryList[0];

        var @event = new DomainEventFake();
        categoryWithEvents.RaiseEvent(@event);

        var eventHandlerMock = new Mock<IDomainEventHandler<DomainEventFake>>();

        _fixture.serviceCollection.AddSingleton(eventHandlerMock.Object);

        await dbContext.AddRangeAsync(exampleCategoryList);

        var unitOfWork =_fixture.GetUnitOfWork(dbContext);
        await unitOfWork.Commit(CancellationToken.None);

        // Usar o mesmo contexto para verificar os dados salvos
        var savedCategories = await dbContext
            .Categories.AsNoTracking()
            .ToListAsync();

        savedCategories.Should().HaveCount(exampleCategoryList.Count);
        foreach (var savedCategory in savedCategories)
        {
            var exampleCategory = exampleCategoryList.Find(x =>
                x.Id == savedCategory.Id
            );
            exampleCategory.Should().NotBeNull();
            savedCategory.Name.Should().Be(exampleCategory!.Name);
            savedCategory.Description.Should().Be(exampleCategory.Description);
            savedCategory.IsActive.Should().Be(exampleCategory.IsActive);
            savedCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        eventHandlerMock.Verify(
            x => x.HandleAsync(It.IsAny<DomainEventFake>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        categoryWithEvents.Events.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(exampleCategoryList);

        var unitOfWork = _fixture.GetUnitOfWork();
        await unitOfWork.Rollback(CancellationToken.None);

        // Usar o mesmo contexto para verificar os dados salvos
        var savedCategories = await dbContext
            .Categories.AsNoTracking()
            .ToListAsync();

        savedCategories.Should().BeEmpty();
    }
}
