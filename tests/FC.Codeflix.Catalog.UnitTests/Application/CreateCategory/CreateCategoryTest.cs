using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async Task CreateCategory()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUowMock();
        var useCase = new UseCase.CreateCategory(
            repoMock.Object,
            uowMock.Object
        );
        var input = _fixture.GetInput();
        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(
            repository =>
                repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        uowMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategoryAggregate))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async Task ThrowWhenCantInstantiateCategoryAggregate(
        UseCase.CreateCategoryInput input,
        string exceptionMessage
    )
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUowMock();
        var useCase = new UseCase.CreateCategory(
            repoMock.Object,
            uowMock.Object
        );
        Func<Task> action = async () =>
            await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }

    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<object[]>();

        // Nome não pode ser menor que 3 caracteres
        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name = "ab";
        invalidInputList.Add(
            new object[]
            {
                invalidInputShortName,
                $"{nameof(invalidInputShortName.Name)} must be at least 3 characters long.",
            }
        );
        // Nome não pode ser maior que 255 caracteres
        var invalidInputLongName = fixture.GetInput();
        invalidInputLongName.Name = new string('a', 256);
        invalidInputList.Add(
            new object[]
            {
                invalidInputLongName,
                $"{nameof(invalidInputLongName.Name)} must be less or equal 255 characters.",
            }
        );
        // Nome não pode ser nullo
        var invalidInputNullName = fixture.GetInput();
        invalidInputNullName.Name = null!;
        invalidInputList.Add(
            new object[]
            {
                invalidInputNullName,
                $"{nameof(invalidInputNullName.Name)} cannot be null or empty.",
            }
        );

        return invalidInputList;
    }
}
