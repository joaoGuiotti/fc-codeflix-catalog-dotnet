using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using Moq;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUowMock();
        var validCategory = _fixture.GetExampleCategory();
        var input = new DeleteCategoryInput(validCategory.Id);

        repositoryMock.Setup(x => x.Get(
            validCategory.Id,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(validCategory);


        var useCase = new UseCase.DeleteCategory(
            repositoryMock.Object,
            uowMock.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Get(
            validCategory.Id,
            It.IsAny<CancellationToken>()
        ), Times.Once);

        repositoryMock.Verify(x => x.Delete(
            validCategory,
            It.IsAny<CancellationToken>()
        ), Times.Once);

        uowMock.Verify(x => x.Commit(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowNotFoundExceptionWhenCategoryDoesNotExist))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task ThrowNotFoundExceptionWhenCategoryDoesNotExist()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUowMock();
        var categoryGuid = Guid.NewGuid();
        var input = new DeleteCategoryInput(categoryGuid);

        repositoryMock.Setup(x => x.Get(
            categoryGuid,
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException($"Category '{categoryGuid}' not found."));


        var useCase = new UseCase.DeleteCategory(
            repositoryMock.Object,
            uowMock.Object
        );

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(x => x.Get(
            categoryGuid,
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

}
