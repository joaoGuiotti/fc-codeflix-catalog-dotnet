namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = Catalog.Domain.Entity;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _fixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture) =>
        _fixture = categoryTestFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validCategory = _fixture.GetValidCategory();

        var dateTimeBefore = DateTime.Now;
        var category = DomainEntity.Category.Create(
            validCategory.Name,
            validCategory.Description
        );
        var dateTimeAfter = DateTime.Now;

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().Be(true);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validaData = new
        {
            Name = "Category name",
            Description = "Category description",
        };

        var dateTimeBefore = DateTime.Now;
        var category = DomainEntity.Category.Create(
            validaData.Name,
            validaData.Description,
            isActive
        );
        var dateTimeAfter = DateTime.Now;

        category.Should().NotBeNull();
        category.Name.Should().Be(validaData.Name);
        category.Description.Should().Be(validaData.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        category.CreatedAt.Should().BeAfter(dateTimeBefore);
        category.CreatedAt.Should().BeBefore(dateTimeAfter);
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void ThrowWhenNameIsEmptyOrNull(string name)
    {
        var validCategory = _fixture.GetValidCategory();
        Action action = () =>
            DomainEntity.Category.Create(name!, validCategory.Description);
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name cannot be null or empty.");
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsLessThanThreeCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("ab")]
    [InlineData("a")]
    public void ThrowWhenNameIsLessThanThreeCharacters(string name)
    {
        Action action = () =>
            DomainEntity.Category.Create(name, "Category Description");
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name must be at least 3 characters long.");
    }

    [Fact(DisplayName = nameof(ThrowWhenNameisGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void ThrowWhenNameisGreaterThan255Characters()
    {
        var invalidName = String.Join(
            null,
            Enumerable.Range(0, 256).Select(_ => "a")
        );
        Action action = () =>
            DomainEntity.Category.Create(invalidName, "Description");
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name must be less or equal 255 characters.");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var category = DomainEntity.Category.Create(
            "Category Name",
            "Category Description",
            false
        );
        category.Activate();
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var category = _fixture.GetValidCategory();
        category.Deactivate();
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = _fixture.GetValidCategory();
        var newValues = new
        {
            Name = "Updated Name",
            Description = "Updated Description",
        };

        category.Update(newValues.Name, newValues.Description);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = _fixture.GetValidCategory();
        var old_description = category.Description;
        var newName = "Updated Name";

        category.Update(newName);

        category.Name.Should().Be(newName);
        category.Description.Should().Be(old_description);
    }

    [Fact]
    [Trait("Domain", "Category - Aggregates")]
    public void ThrowErrorWhenUpdateWithEmptyName()
    {
        var category = _fixture.GetValidCategory();
        Action action1 = () => category.Update(string.Empty);
        action1
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name cannot be null or empty.");

        Action action2 = () => category.Update("  ", "New Description");
        action2
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name cannot be null or empty.");
    }

    [Fact]
    [Trait("Domain", "Category - Aggregates")]
    public void ThrowErrorWhenDescriptionGreaterThan10_000()
    {
        var invalidDescription = String.Join(
            null,
            Enumerable.Range(0, 10_001).Select(_ => "a")
        );
        ;
        Action action = () =>
            DomainEntity.Category.Create("Category Name", invalidDescription);
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage(
                "Description must be less or equal 10.000 caracteres."
            );
    }
}
