using Moq;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var category = _fixture.GetExampleCategory();

        repoMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(category);

        var useCase = new UseCase.GetCategory(repoMock.Object);
        var input = new UseCase.GetCategoryInput(category.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(
            repository => repository.Get(
                It.Is<Guid>(id => id == input.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(category.Name);
        output.Description.Should().Be(category.Description);
        output.IsActive.Should().Be(category.IsActive);
        output.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repoMock = _fixture.GetRepositoryMock();

        var useCase = new UseCase.GetCategory(repoMock.Object);
        var input = new UseCase.GetCategoryInput(Guid.NewGuid());

        repoMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((Category?)null!);

        Func<Task> action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Category with ID {input.Id} not found.");
    }

}
