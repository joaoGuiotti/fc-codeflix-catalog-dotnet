using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixtureCollection))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var category = _fixture.GetExampleCategory();

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var repo = new CategoryRepository(dbContext);
        var useCase = new UseCase.GetCategory(repo);

        var input = new GetCategoryInput(category.Id);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(category.Id);
        output.Name.Should().Be(category.Name);
        output.Description.Should().Be(category.Description);
        output.IsActive.Should().Be(category.IsActive);
        output.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);

        var exampleCategory = _fixture.GetExampleCategory();

        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var useCase = new UseCase.GetCategory(repo);
        var input = new GetCategoryInput(Guid.NewGuid());

        Func<Task> action = async () =>
            await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Category with ID {input.Id} not found.");
    }
}
