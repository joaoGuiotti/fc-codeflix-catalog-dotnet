using System.Text;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideo()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var categoryRepoMock = _fixture.GetCategoryRepoMock();

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            categoryRepoMock.Object,
            _fixture.GetGenreRepoMock().Object,
            _fixture.GetCastMemberRepoMock().Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategories))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithCategories()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var categoryRepoMock = _fixture.GetCategoryRepoMock();
        var categoriesIds = _fixture.GetListGuids();

        categoryRepoMock.Setup(x =>
            x.GetIdsListById(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoriesIds);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            categoryRepoMock.Object,
            _fixture.GetGenreRepoMock().Object,
            _fixture.GetCastMemberRepoMock().Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(categoriesIds: categoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEquivalentTo(categoriesIds);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Categories.All(categoriesIds.Contains)
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(CreateVideoThrowsWhenNotFoundCategory))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoThrowsWhenNotFoundCategory()
    {
        var repoMock = _fixture.GetRepoMock();
        var categoryRepoMock = _fixture.GetCategoryRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var categoriesIds = _fixture.GetListGuids();

        categoryRepoMock.Setup(x =>
            x.GetIdsListById(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoriesIds);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            categoryRepoMock.Object,
            _fixture.GetGenreRepoMock().Object,
            _fixture.GetCastMemberRepoMock().Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var exampleInvalidCategoryId = Guid.NewGuid();

        var input = _fixture.CreateVideoInput(
            categoriesIds: [.. categoriesIds, exampleInvalidCategoryId]
        );

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {exampleInvalidCategoryId}.");

        categoryRepoMock.VerifyAll();
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInputs))]
    [Trait("Video", "Create Video - Use Cases")]
    [MemberData(
        nameof(CreateVideoTestGenerator.GenerateInvalidVideoInputs),
        parameters: 12,
        MemberType = typeof(CreateVideoTestGenerator)
    )]
    public async Task CreateVideoThrowsWithInvalidInputs(
        UseCase.CreateVideoInput input,
        string expectedErrorMessage
    )
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            _fixture.GetGenreRepoMock().Object,
            _fixture.GetCastMemberRepoMock().Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var expectedAssertion = await action.Should()
            .ThrowAsync<EntityValidationException>();

        expectedAssertion.WithMessage("There are validation errors.")
            .Which.Errors.Should().Contain(x => x.ErrorMessage == expectedErrorMessage);

        repoMock.Verify(x =>
                x.Insert(
                    It.IsAny<DomainEntity.Video>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }


    [Fact(DisplayName = nameof(CreateVideoWithGenres))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithGenres()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var genresIds = _fixture.GetListGuids();

        genreRepoMock.Setup(x =>
            x.GetIdsListById(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(genresIds);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            _fixture.GetCastMemberRepoMock().Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(genresIds: genresIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEquivalentTo(genresIds);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Genres.All(genresIds.Contains)
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(CreateVideoThrowsWhenNotFoundGenre))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoThrowsWhenNotFoundGenre()
    {
        var repoMock = _fixture.GetRepoMock();
        var categoryRepoMock = _fixture.GetCategoryRepoMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var genresIds = _fixture.GetListGuids();

        genreRepoMock.Setup(x =>
            x.GetIdsListById(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(genresIds);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            categoryRepoMock.Object,
            genreRepoMock.Object,
            _fixture.GetCastMemberRepoMock().Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var exampleInvalidGenreId = Guid.NewGuid();

        var input = _fixture.CreateVideoInput(
            genresIds: [.. genresIds, exampleInvalidGenreId]
        );

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {exampleInvalidGenreId}.");

        categoryRepoMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembers))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithCastMembers()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var castMembersIds = _fixture.GetListGuids();

        castMemberRepoMock.Setup(x =>
            x.GetIdsListById(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(castMembersIds);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(castMembersIds: castMembersIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEmpty();
        output.CastMembersIds.Should().BeEquivalentTo(castMembersIds);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.CastMembers.All(castMembersIds.Contains)
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(CreateVideoThrowsWhenNotFoundCastMember))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoThrowsWhenNotFoundCastMember()
    {
        var repoMock = _fixture.GetRepoMock();
        var categoryRepoMock = _fixture.GetCategoryRepoMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var castMembersIds = _fixture.GetListGuids();

        castMemberRepoMock.Setup(x =>
            x.GetIdsListById(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(castMembersIds);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            categoryRepoMock.Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            _fixture.GetStorageServiceMock().Object,
            uowMock.Object
        );
        var exampleInvalidCastMemberId = Guid.NewGuid();

        var input = _fixture.CreateVideoInput(
            castMembersIds: [.. castMembersIds, exampleInvalidCastMemberId]
        );

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related castMember id (or ids) not found: {exampleInvalidCastMemberId}.");

        categoryRepoMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithThumb()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var thumbFile = _fixture.GetValidImageFileInput();
        var expectedThumbName = $"thumb.{thumbFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(thumb: thumbFile);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEmpty();
        output.CastMembersIds.Should().BeEmpty();
        output.Thumb.Should().Be(expectedThumbName);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithBanner()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var bannerFile = _fixture.GetValidImageFileInput();

        var expectedBannerName = $"thumb.{bannerFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedBannerName);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(banner: bannerFile);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEmpty();
        output.CastMembersIds.Should().BeEmpty();
        output.Banner.Should().Be(expectedBannerName);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithThumbHalf()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var thumbHalfFile = _fixture.GetValidImageFileInput();

        var expectedThumbHalfName = $"thumb.{thumbHalfFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbHalfName);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(thumbHalf: thumbHalfFile);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.ThumbHalf.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithMedia()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var mediaFile = _fixture.GetValidMediaFileInput();
        var expectedMediaName = $"media.{mediaFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains($"-media.{mediaFile.Extension}")),
                It.Is<Stream>(s => s == mediaFile.FileStream),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedMediaName);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(media: mediaFile);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Media.Should().Be(expectedMediaName);

        storageServiceMock.Verify(x => x.Upload(
            It.Is<string>(s => s == $"{output.Id}-media.{mediaFile.Extension}"),
            It.Is<Stream>(s => s == mediaFile.FileStream),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Media != null &&
                video.Media.FilePath == expectedMediaName
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoWithTrailer()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var trailerFile = _fixture.GetValidMediaFileInput();
        var expectedTrailerName = $"trailer.{trailerFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains($"-trailer.{trailerFile.Extension}")),
                It.Is<Stream>(s => s == trailerFile.FileStream),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedTrailerName);

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(trailer: trailerFile);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Trailer.Should().Be(expectedTrailerName);

        storageServiceMock.Verify(x => x.Upload(
            It.Is<string>(s => s == $"{output.Id}-trailer.{trailerFile.Extension}"),
            It.Is<Stream>(s => s == trailerFile.FileStream),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Trailer != null &&
                video.Trailer.FilePath == expectedTrailerName
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoThrowsWhenAnErrorOccursOnUploadMedias))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoThrowsWhenAnErrorOccursOnUploadMedias()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var thumbFile = _fixture.GetValidImageFileInput();
        var bannerFile = _fixture.GetValidImageFileInput();
        var thumbHalfFile = _fixture.GetValidImageFileInput();
        var mediaFile = _fixture.GetValidMediaFileInput();
        var trailerFile = _fixture.GetValidMediaFileInput();

        var expectedThumbName = $"thumb.{thumbFile.Extension}";
        var expectedBannerName = $"banner.{bannerFile.Extension}";
        var expectedThumbHalfName = $"thumbhalf.{thumbHalfFile.Extension}";
        var expectedMediaName = $"media.{mediaFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-thumb.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-banner.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedBannerName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-thumbhalf.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbHalfName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-media.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedMediaName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-trailer.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new Exception("Error uploading trailer"));

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(
            thumb: thumbFile,
            banner: bannerFile,
            thumbHalf: thumbHalfFile,
            media: mediaFile,
            trailer: trailerFile
        );

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Error uploading trailer");

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedThumbName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedBannerName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedThumbHalfName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedMediaName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(4));

        repoMock.Verify(x => x.Insert(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);

        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(CreateVideoThrowsWhenAnErrorOccursOnCommitAndClearsUploadedMedias))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoThrowsWhenAnErrorOccursOnCommitAndClearsUploadedMedias()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genreRepoMock = _fixture.GetGenreRepoMock();
        var castMemberRepoMock = _fixture.GetCastMemberRepoMock();
        var storageServiceMock = _fixture.GetStorageServiceMock();

        var thumbFile = _fixture.GetValidImageFileInput();
        var bannerFile = _fixture.GetValidImageFileInput();
        var thumbHalfFile = _fixture.GetValidImageFileInput();
        var mediaFile = _fixture.GetValidMediaFileInput();
        var trailerFile = _fixture.GetValidMediaFileInput();

        var expectedThumbName = $"thumb.{thumbFile.Extension}";
        var expectedBannerName = $"banner.{bannerFile.Extension}";
        var expectedThumbHalfName = $"thumbhalf.{thumbHalfFile.Extension}";
        var expectedMediaName = $"media.{mediaFile.Extension}";
        var expectedTrailerName = $"trailer.{trailerFile.Extension}";

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-thumb.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-banner.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedBannerName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-thumbhalf.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbHalfName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-media.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedMediaName);

        storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(s => s.Contains("-trailer.")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedTrailerName);

        uowMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Commit error"));

        var useCase = new UseCase.CreateVideo(
            repoMock.Object,
            _fixture.GetCategoryRepoMock().Object,
            genreRepoMock.Object,
            castMemberRepoMock.Object,
            storageServiceMock.Object,
            uowMock.Object
        );
        var input = _fixture.CreateVideoInput(
            thumb: thumbFile,
            banner: bannerFile,
            thumbHalf: thumbHalfFile,
            media: mediaFile,
            trailer: trailerFile
        );

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Commit error");

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedThumbName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedBannerName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedThumbHalfName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedMediaName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(name => name == expectedTrailerName),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(5));
    }
}
