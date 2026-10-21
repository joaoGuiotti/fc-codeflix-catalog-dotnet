using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixtureCollection))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var exampleCategory = _fixture.GetExampleCategory();
        var input = new DeleteCategoryInput(exampleCategory.Id);

        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);

        await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();

        var useCase = new UseCase.DeleteCategory(repo, uow);
        await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await dbContext.Categories.FindAsync(
            exampleCategory.Id
        );
        dbCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteCategoryThrowsWhenNotFound))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategoryThrowsWhenNotFound()
    {
        var exampleCategory = _fixture.GetExampleCategory();
        var input = new DeleteCategoryInput(exampleCategory.Id);

        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);

        var useCase = new UseCase.DeleteCategory(repo, uow);
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{exampleCategory.Id}' not found.");
    }
}
