using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
        => _fixture = fixture;


    [Theory(DisplayName = nameof(UpdateSuccess))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoryToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateSuccess(
        Category category,
        UpdateCategoryInput input
    )
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUowMock();

        repositoryMock.Setup(x =>
            x.Get(
                category.Id,
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(category);

        uowMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()));

        // Act
        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            uowMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);
        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive!.Value);

        repositoryMock.Verify(x => x.Get(
            category.Id,
            It.IsAny<CancellationToken>()
        ), Times.Once);

        repositoryMock.Verify(x => x.Update(
            It.Is<Category>(category =>
                category.Id == input.Id
                && category.Name == input.Name
                && category.Description == input.Description
                && category.IsActive == input.IsActive
                && category.CreatedAt == category.CreatedAt
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUowMock();
        var input = _fixture.GetValidInput();

        repositoryMock.Setup(x => x.Get(
            input.Id,
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException($"Category '{input.Id}' not found"));
        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(x => x.Get(
            input.Id,
            It.IsAny<CancellationToken>())
        , Times.Once);
    }

    // Update Category wihtout passing IsActive
    [Fact(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async Task UpdateCategoryWithoutIsActive()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUowMock();
        var validCategory = _fixture.GetValidCategory();
        var input = new UpdateCategoryInput(
            validCategory.Id,
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        repositoryMock.Setup(x => x.Get(
            validCategory.Id,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(validCategory);

        uowMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()));

        // Act
        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            uowMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);
        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(validCategory.IsActive);

        repositoryMock.Verify(x => x.Get(
            validCategory.Id,
            It.IsAny<CancellationToken>()
        ), Times.Once);

        repositoryMock.Verify(x => x.Update(
            It.Is<Category>(category =>
                category.Id == input.Id
                && category.Name == input.Name
                && category.Description == input.Description
                && category.IsActive == validCategory.IsActive
                && category.CreatedAt == validCategory.CreatedAt
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task ThrowWhenCantUpdateCategory(
        UpdateCategoryInput input,
        string expectedExceptionMessage
    )
    {
        {
            var exampleCategory = _fixture.GetValidCategory();
            input.Id = exampleCategory.Id;
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUowMock();
            repositoryMock.Setup(x => x.Get(
                exampleCategory.Id,
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(exampleCategory);
            var useCase = new UseCase.UpdateCategory(
                repositoryMock.Object,
                unitOfWorkMock.Object
            );

            var task = async ()
                => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<EntityValidationException>()
                .WithMessage(expectedExceptionMessage);

            repositoryMock.Verify(x => x.Get(
                exampleCategory.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
        }
    }

}