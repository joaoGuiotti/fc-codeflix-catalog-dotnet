using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
        await dbContext.AddRangeAsync(exampleCategoryList);

        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
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
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data.EF", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(exampleCategoryList);

        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
        await unitOfWork.Rollback(CancellationToken.None);

        // Usar o mesmo contexto para verificar os dados salvos
        var savedCategories = await dbContext
            .Categories.AsNoTracking()
            .ToListAsync();

        savedCategories.Should().BeEmpty();
    }
}
