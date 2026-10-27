using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryInputValidationTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryInputValidationTest(GetCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "GetCategoryInputValidation - Use Cases")]
    public void ValidationOk()
    {
        var validInput = new GetCategoryInput(Guid.NewGuid());
        var validator = new GetCategoryInputValidator();

        var validateResult = validator.Validate(validInput);
        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ValidationFailWhenIdIsEmpty))]
    [Trait("Application", "GetCategoryInputValidation - Use Cases")]
    public void ValidationFailWhenIdIsEmpty()
    {
        var invalidInput = new GetCategoryInput(Guid.Empty);
        var validator = new GetCategoryInputValidator();

        var validateResult = validator.Validate(invalidInput);
        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeFalse();
        validateResult.Errors.Should().HaveCount(1);
        validateResult
            .Errors[0]
            .ErrorMessage.Should()
            .Be("Category ID is required.");
    }
}
