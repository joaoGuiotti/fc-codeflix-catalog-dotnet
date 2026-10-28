using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.CreateGenre;

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
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();

        var useCase = new UseCase.CreateGenre(repoMock.Object, uowMock.Object, categoryRepoMock.Object);

        var input = _fixture.GetValidInput();
        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ));

        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateGenreWithCategories))]
    [Trait("Genre", "Create Genre - Use Cases")]
    public async Task CreateGenreWithCategories()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();

        var useCase = new UseCase.CreateGenre(repoMock.Object, uowMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetInputWithCategories();

        categoryRepoMock.Setup(x => x.GetIdsListById(
              It.IsAny<List<Guid>>(),
              It.IsAny<CancellationToken>()
          )).ReturnsAsync((IReadOnlyList<Guid>)input.CategoriesIds!);

        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(input.CategoriesIds!.Count());
        input.CategoriesIds?.ForEach(id =>
           output.Categories.Should().Contain(relation => relation.Id == id)
       );
    }


    [Fact(DisplayName = nameof(ThrowError))]
    [Trait("Genre", "Create Genre - Use Cases")]
    public async Task ThrowError()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();

        var useCase = new UseCase.CreateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

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
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();

        var input = _fixture.GetInputWithCategories();
        var exampleGuid = input.CategoriesIds![^1];

        categoryRepoMock.Setup(x => x.GetIdsListById(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            input.CategoriesIds
                .FindAll(x => x != exampleGuid)
                .AsReadOnly()
        );

        var useCase = new UseCase.CreateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Category id (or ids) not found: {exampleGuid}.");

        categoryRepoMock.Verify(
            x => x.GetIdsListById(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
