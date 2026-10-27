using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixtureCollection))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var validGenre = _fixture.GetValidGenre();

        var dateTimeBefore = DateTime.Now;
        var genre = DomainEntity.Genre.Create(
            validGenre.Name,
            validGenre.IsActive
        );
        var dateTimeAfter = DateTime.Now;

        genre.Should().NotBeNull();
        genre.Id.Should().NotBe(default(Guid));
        genre.Name.Should().NotBeNull(validGenre.Name);
        genre.IsActive.Should().Be(validGenre.IsActive);
        genre.CreatedAt.Should().NotBe(default(DateTime));
        genre.CreatedAt.Should().BeOnOrAfter(dateTimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(dateTimeAfter);
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void ThrowWhenNameIsEmptyOrNull(string name)
    {
        var validCategory = _fixture.GetValidGenre();
        Action action = () =>
            DomainEntity.Genre.Create(name);
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name cannot be null or empty.");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Activate()
    {
        var category = DomainEntity.Genre.Create(
            "Genre Name",
            false
        );
        category.Activate();
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Deactivate()
    {
        var category = _fixture.GetValidGenre();
        category.Deactivate();
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {
        var category = _fixture.GetValidGenre();
        var nameToUpdate = _fixture.GetValidName();
        category.Update(nameToUpdate);
        category.Name.Should().Be(nameToUpdate);
    }

    [Theory(DisplayName = nameof(ThrowWhenNameToShort))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("ab")]
    [InlineData("a")]
    public void ThrowWhenNameToShort(string name)
    {
        Action action = () => DomainEntity.Genre.Create(name);
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name must be at least 3 characters long.");
    }

    [Fact(DisplayName = nameof(ThrowWhenNameisGreaterThan255Characters))]
    [Trait("Domain", "Genre - Aggregates")]
    public void ThrowWhenNameisGreaterThan255Characters()
    {
        var invalidName = String.Join(
            null,
            Enumerable.Range(0, 256).Select(_ => "a")
        );
        Action action = () =>
            DomainEntity.Genre.Create(invalidName);
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name must be less or equal 255 characters.");
    }


    [Fact(DisplayName = nameof(AddCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategories()
    {
        var genre = _fixture.GetValidGenre();
        var categoryGuid = Guid.NewGuid();

        genre.AddCategory(categoryGuid);

        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryGuid);
    }


    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveCategory()
    {
        int categoriesLength = 15;
        var genre = _fixture.GetValidGenreWithCategories(categoriesLength);
        var categoryToRemove = genre.Categories[10];

        genre.RemoveCategory(categoryToRemove);

        genre.Categories.Should().HaveCount((categoriesLength - 1));
        genre.Categories.Should().NotContain(categoryToRemove);
    }


    [Fact(DisplayName = nameof(RemoveAllCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveAllCategory()
    {
        int categoriesLength = 15;
        var genre = _fixture.GetValidGenreWithCategories(categoriesLength);

        genre.RemoveAllCategories();

        genre.Categories.Should().HaveCount(0);
    }
}
