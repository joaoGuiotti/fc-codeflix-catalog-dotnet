
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Genre", "Update Genre - Use Cases")]
    public async Task UpdateGenre()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var input = _fixture.GetValidInput(exampleGenre.Id);

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.isActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);

        repoMock.Verify(
            x => x.Get(
                It.Is<Guid>(id => id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.Genre>(genre =>
                    genre.Id == exampleGenre.Id
                    && genre.Name == input.Name
                    && genre.IsActive == input.isActive
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(UpdateGenreActivatingAndDeactivating))]
    [Trait("Genre", "Update Genre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreActivatingAndDeactivating(bool currentIsActive)
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(isActive: currentIsActive);
        var expectedIsActive = !currentIsActive;
        var input = new UpdateGenreInput(
            exampleGenre.Id,
            _fixture.GetValidName(),
            expectedIsActive
        );

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(expectedIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);

        repoMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.Genre>(genre =>
                    genre.Id == exampleGenre.Id
                    && genre.Name == input.Name
                    && genre.IsActive == expectedIsActive
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(UpdateGenreWithoutProvidingIsActive))]
    [Trait("Genre", "Update Genre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreWithoutProvidingIsActive(bool currentIsActive)
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(isActive: currentIsActive);
        var input = new UpdateGenreInput(
            exampleGenre.Id,
            _fixture.GetValidName()
        );

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(currentIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);

        repoMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.Genre>(genre =>
                    genre.Id == exampleGenre.Id
                    && genre.Name == input.Name
                    && genre.IsActive == currentIsActive
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(UpdateGenreWithNewCategoriesIds))]
    [Trait("Genre", "Update Genre - Use Cases")]
    public async Task UpdateGenreWithNewCategoriesIds()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: _fixture.GetRandomIdsList()
        );
        var newCategoriesIds = _fixture.GetRandomIdsList();
        var input = _fixture.GetValidInputWithCategories(
            exampleGenre.Id,
            newCategoriesIds
        );

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        categoryRepoMock.Setup(x => x.GetIdsListById(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(newCategoriesIds.AsReadOnly());

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.isActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(newCategoriesIds.Count);
        newCategoriesIds.ForEach(categoryId =>
            output.Categories.Should().Contain(categoryId)
        );

        repoMock.Verify(
            x => x.Get(
                It.Is<Guid>(id => id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        categoryRepoMock.Verify(
            x => x.GetIdsListById(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.Genre>(genre =>
                    genre.Id == exampleGenre.Id
                    && genre.Name == input.Name
                    && genre.IsActive == input.isActive
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Genre", "Update Genre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput();

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == input.Id),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException($"Genre '{input.Id}' not found"));

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{input.Id}' not found");

        repoMock.Verify(
            x => x.Get(
                It.Is<Guid>(id => id == input.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
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

    [Theory(DisplayName = nameof(ThrowWhenInvalidName))]
    [Trait("Genre", "Update Genre - Use Cases")]
    [InlineData("", "Name cannot be null or empty.")]
    [InlineData("  ", "Name cannot be null or empty.")]
    [InlineData(null, "Name cannot be null or empty.")]
    [InlineData("a", "Name must be at least 3 characters long.")]
    [InlineData("ab", "Name must be at least 3 characters long.")]
    public async Task ThrowWhenInvalidName(
        string? invalidName,
        string expectedExceptionMessage
    )
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var input = new UpdateGenreInput(
            exampleGenre.Id,
            invalidName!,
            _fixture.GetRandomBoolean()
        );

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);

        repoMock.Verify(
            x => x.Get(
                It.Is<Guid>(id => id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
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

    [Fact(DisplayName = nameof(ThrowWhenNameIsTooLong))]
    [Trait("Genre", "Update Genre - Use Cases")]
    public async Task ThrowWhenNameIsTooLong()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var input = new UpdateGenreInput(
            exampleGenre.Id,
            _fixture.GetInvalidNameTooLong(),
            _fixture.GetRandomBoolean()
        );

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("Name must be less or equal 255 characters.");

        repoMock.Verify(
            x => x.Get(
                It.Is<Guid>(id => id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
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

    [Fact(DisplayName = nameof(ThrowWhenRelatedCategoryNotFound))]
    [Trait("Genre", "Update Genre - Use Cases")]
    public async Task ThrowWhenRelatedCategoryNotFound()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryrepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var input = _fixture.GetValidInputWithCategories(exampleGenre.Id);
        var exampleCategoryGuid = input.CategoriesIds![^1];

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        categoryRepoMock.Setup(x => x.GetIdsListById(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            input.CategoriesIds
                .FindAll(id => id != exampleCategoryGuid)
                .AsReadOnly()
        );

        var useCase = new UseCase.UpdateGenre(
            repoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Category id (or ids) not found: {exampleCategoryGuid}.");

        repoMock.Verify(
            x => x.Get(
                It.Is<Guid>(id => id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        categoryRepoMock.Verify(
            x => x.GetIdsListById(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
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
