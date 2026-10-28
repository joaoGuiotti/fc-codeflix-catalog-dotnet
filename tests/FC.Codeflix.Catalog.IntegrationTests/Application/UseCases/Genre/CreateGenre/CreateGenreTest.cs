using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Genre", "Create Genre - Use Cases")]
    public async Task CreateGenre()
    {
        var useCase = _fixture.CreateGenreUseCase();

        var input = _fixture.GetValidInput();
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(ThrowError))]
    [Trait("Category", "Create Genre - Use Cases")]
    public async Task ThrowError()
    {
        var useCase = _fixture.CreateGenreUseCase();

        var input = new CreateGenreInput(_fixture.GetInvalidNameTooLong());
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("Name must be less or equal 255 characters.");
    }

    [Fact(DisplayName = nameof(ThrowErrorWhenCreateWithInexistentsCategories))]
    [Trait("Genre", "Create Genre - Use Cases")]
    public async Task ThrowErrorWhenCreateWithInexistentsCategories()
    {
        var dbContext = _fixture.CreateDbContext();
        var useCase = _fixture.CreateGenreUseCase(dbContext);

        var input = _fixture.GetInputWithCategories();
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var notFoundIdsAsString = string.Join(", ", input.CategoriesIds!);
        await action.Should()
            .ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Category id (or ids) not found: {notFoundIdsAsString}.");

        dbContext.Genres.Count().Should().Be(0);
    }

    [Fact(DisplayName = nameof(CreateGenreWithExistingCategories))]
    [Trait("Genre", "Create Genre - Use Cases")]
    public async Task CreateGenreWithExistingCategories()
    {
        var dbContext = _fixture.CreateDbContext();

        var categories = new List<Domain.Entity.Category>([
            DomainEntity.Category.Create("Category01", "Desription", true),
            DomainEntity.Category.Create("Category02", "Desription 02", true),
        ]);

        await dbContext.Categories.AddRangeAsync(categories, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        dbContext.Categories.Count().Should().Be(categories.Count);

        var categoriesIds = categories.Select(x => x.Id).ToList();
        var input = _fixture.GetInputWithCategories(categoriesIds);
        var useCase = _fixture.CreateGenreUseCase(dbContext);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Categories.Should().HaveCount(categories.Count);
        foreach (var categoryOutput in output.Categories)
        {
            var expectedCategory = categories.Single(x => x.Id == categoryOutput.Id);
        }

        dbContext.Genres.Count().Should().Be(1);
        var genre = await dbContext.Genres.FindAsync([output.Id]);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(input.Name);
        genre.Categories.Should().HaveCount(categories.Count);
        categoriesIds.ForEach(categoryId
                => genre.Categories.Contains(categoryId)
                    .Should().BeTrue()
        );
    }
}
