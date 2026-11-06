using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideo;

[Collection(nameof(ListVideoTestFixture))]
public class ListVideoTest
{
    private readonly ListVideoTestFixture _fixture;
    private readonly UseCase.ListVideo _useCase;
    private readonly Mock<IVideoRepository> _videoRepositoryMock = new Mock<IVideoRepository>();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new Mock<ICategoryRepository>();
    private readonly Mock<IGenreRepository> _genreRepositoryMock = new Mock<IGenreRepository>();
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock = new Mock<ICastMemberRepository>();

    public ListVideoTest(ListVideoTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UseCase.ListVideo(
            _videoRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _castMemberRepositoryMock.Object
        );

    }

    [Fact(DisplayName = nameof(ListVideos))]
    [Trait("Application", "ListVideo - Use Cases")]
    public async Task ListVideos()
    {
        var exampleVideosList = _fixture.GetExampleVideosList();
        var input = _fixture.GetValidInput();

        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(search =>
                search.Page == input.Page
                && search.PerPage == input.PerPage
                && search.Search == input.Search
                && search.Sort == input.Sort
                && search.SortDir == input.SortDir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                total: exampleVideosList.Count,
                items: exampleVideosList
            )
        );

        PaginatedListOutput<VideoModelOutput> output =
            await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem!.Id);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Published.Should().Be(exampleItem.Published);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.Duration.Should().Be(exampleItem.Duration);
            outputItem.Rating.Should().Be(exampleItem.Rating);
            outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
            outputItem.Opened.Should().Be(exampleItem.Opened);
            outputItem.Thumb.Should().Be(exampleItem.Thumb!.Path);
            outputItem.Banner.Should().Be(exampleItem.Banner!.Path);
            outputItem.ThumbHalf.Should().Be(exampleItem.ThumbHalf!.Path);
            outputItem.Media.Should().Be(exampleItem.Media!.FilePath);
            outputItem.Trailer.Should().Be(exampleItem.Trailer!.FilePath);
            var outputItemCategoriesIds = outputItem.Categories.Select(dto => dto.Id).ToList();
            outputItemCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
            var outputItemGenresIds = outputItem.Genres.Select(dto => dto.Id).ToList();
            outputItemGenresIds.Should().BeEquivalentTo(exampleItem.Genres);
            var outputItemCastMembersIds = outputItem.CastMembers.Select(dto => dto.Id).ToList();
            outputItemCastMembersIds.Should().BeEquivalentTo(exampleItem.CastMembers);
        });
        _videoRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ListVideosWithRelations))]
    [Trait("Application", "ListVideo - Use Cases")]
    public async Task ListVideosWithRelations()
    {
        var (exampleVideosList, examplesCategories, exampleGenres, exampleCastMembers) =
            _fixture.GetExampleVideosListWithRelations();
        var input = _fixture.GetValidInput();
        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(search =>
                search.Page == input.Page
                && search.PerPage == input.PerPage
                && search.Search == input.Search
                && search.Sort == input.Sort
                && search.SortDir == input.SortDir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                total: exampleVideosList.Count,
                items: exampleVideosList
            )
        );
        _categoryRepositoryMock.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list =>
                list.All(examplesCategories.Select(cat => cat.Id).Contains)
                && list.Count == examplesCategories.Count
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(examplesCategories);
        _genreRepositoryMock.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list =>
                list.All(exampleGenres.Select(genre => genre.Id).Contains)
                && list.Count == exampleGenres.Count
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenres);
        _castMemberRepositoryMock.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list =>
                list.All(exampleCastMembers.Select(c => c.Id).Contains)
                && list.Count == exampleCastMembers.Count
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleCastMembers);


        PaginatedListOutput<VideoModelOutput> output =
            await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem!.Id);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Published.Should().Be(exampleItem.Published);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.Duration.Should().Be(exampleItem.Duration);
            outputItem.Rating.Should().Be(exampleItem.Rating);
            outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
            outputItem.Opened.Should().Be(exampleItem.Opened);
            outputItem.Thumb.Should().Be(exampleItem.Thumb!.Path);
            outputItem.Banner.Should().Be(exampleItem.Banner!.Path);
            outputItem.ThumbHalf.Should().Be(exampleItem.ThumbHalf!.Path);
            outputItem.Media.Should().Be(exampleItem.Media!.FilePath);
            outputItem.Trailer.Should().Be(exampleItem.Trailer!.FilePath);
            outputItem.Categories.ToList().ForEach(relation =>
            {
                var exampleCategory = examplesCategories.Find(x => x.Id == relation.Id);
                exampleCategory.Should().NotBeNull();
                relation.Name.Should().Be(exampleCategory!.Name);
            });
            outputItem.Genres.ToList().ForEach(relation =>
            {
                var exampleGenre = exampleGenres.Find(x => x.Id == relation.Id);
                exampleGenre.Should().NotBeNull();
                relation.Name.Should().Be(exampleGenre!.Name);
            });
            outputItem.CastMembers.ToList().ForEach(relation =>
            {
                var exampleCastMember = exampleCastMembers.Find(x => x.Id == relation.Id);
                exampleCastMember.Should().NotBeNull();
                relation.Name.Should().Be(exampleCastMember!.Name);
            });
            var outputItemCastMembersIds = outputItem.CastMembers.Select(dto => dto.Id).ToList();
            outputItemCastMembersIds.Should().BeEquivalentTo(exampleItem.CastMembers);
        });
        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ListVideosWithoutRelationsDoesntCallOtherRepositories))]
    [Trait("Application", "ListVideo - Use Cases")]
    public async Task ListVideosWithoutRelationsDoesntCallOtherRepositories()
    {
        var exampleVideos = _fixture.GetExampleVideosListWithoutRelations();
        var input = _fixture.GetValidInput();

        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(search =>
                search.Page == input.Page
                && search.PerPage == input.PerPage
                && search.Search == input.Search
                && search.Sort == input.Sort
                && search.SortDir == input.SortDir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                total: exampleVideos.Count,
                items: exampleVideos
            )
        );

        PaginatedListOutput<VideoModelOutput> output =
            await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideos.Count);
        output.Items.Should().HaveCount(exampleVideos.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleVideos.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Categories.Should().BeEmpty();
            outputItem.Genres.Should().BeEmpty();
            outputItem.CastMembers.Should().BeEmpty();
        });
        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.Verify(
            x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _genreRepositoryMock.Verify(
            x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereIsNoVideo))]
    [Trait("Application", "ListVideo - Use Cases")]
    public async Task ListReturnsEmptyWhenThereIsNoVideo()
    {
        var exampleVideos = new List<DomainEntity.Video>();
        var input = _fixture.GetValidInput();
        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(search =>
                search.Page == input.Page
                && search.PerPage == input.PerPage
                && search.Search == input.Search
                && search.Sort == input.Sort
                && search.SortDir == input.SortDir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                total: exampleVideos.Count,
                items: exampleVideos
            )
        );

        PaginatedListOutput<VideoModelOutput> output =
            await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().BeEmpty();
        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.Verify(
            x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _genreRepositoryMock.Verify(
            x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(DisplayName = nameof(ListUsesDefaultsWhenInputHasNoParameters))]
    [Trait("Application", "ListVideo - Use Cases")]
    public async Task ListUsesDefaultsWhenInputHasNoParameters()
    {

        _videoRepositoryMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            new SearchOutput<DomainEntity.Video>(
                currentPage: 1,
                perPage: 15,
                total: 0,
                items: new List<DomainEntity.Video>()
            )
        );

        await _useCase.Handle(new UseCase.ListVideoInput(), CancellationToken.None);

        _videoRepositoryMock.Verify(
            x => x.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page == 1
                    && searchInput.PerPage == 15
                    && searchInput.Search == ""
                    && searchInput.Sort == ""
                    && searchInput.SortDir == ESearchOrder.Asc
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}

