using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[Collection(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture _fixture;

    public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
    }

    [Fact(DisplayName = nameof(InsertWithRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertWithRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();

        var categories = _fixture.GetExampleCategoriesList(3);
        categories.ForEach(category => exampleVideo.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categories);

        var genres = _fixture.GetExampleGenresList(3);
        genres.ForEach(genre => exampleVideo.AddGenre(genre.Id));
        await dbContext.Genres.AddRangeAsync(genres);

        var castMembers = _fixture.GetExampleCastMembersList(3);
        castMembers.ForEach(castMember => exampleVideo.AddCastMember(castMember.Id));
        await dbContext.CastMembers.AddRangeAsync(castMembers);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(dbContext);
        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();

        var dbVideosCategories = await assertsDbContext.VideosCategories
            .Where(x => x.VideoId == exampleVideo.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        dbVideosCategories.Should().HaveCount(categories.Count);
        dbVideosCategories.Should().BeEquivalentTo(categories.Select(x => x.Id));

        var dbVideosGenres = await assertsDbContext.VideosGenres
            .Where(x => x.VideoId == exampleVideo.Id)
            .Select(x => x.GenreId)
            .ToListAsync();
        dbVideosGenres.Should().HaveCount(genres.Count);
        dbVideosGenres.Should().BeEquivalentTo(genres.Select(x => x.Id));

        var dbVideosCastMembers = await assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == exampleVideo.Id)
            .Select(x => x.CastMemberId)
            .ToListAsync();
        dbVideosCastMembers.Should().HaveCount(castMembers.Count);
        dbVideosCastMembers.Should().BeEquivalentTo(castMembers.Select(x => x.Id));
    }

    [Fact(DisplayName = nameof(InsertWithImagesAndMedias))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertWithImagesAndMedias()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideoWithAllProperties();
        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);

        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media!.Id.Should().Be(exampleVideo.Media!.Id);
        dbVideo.Media.FilePath.Should().Be(exampleVideo.Media.FilePath);
        dbVideo.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        dbVideo.Media.Status.Should().Be(exampleVideo.Media.Status);

        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.Id.Should().Be(exampleVideo.Trailer!.Id);
        dbVideo.Trailer.FilePath.Should().Be(exampleVideo.Trailer.FilePath);
        dbValueMediaCheck(dbVideo.Trailer, exampleVideo.Trailer);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        var exampleVideosList = _fixture.GetExampleVideosListWithoutRelations(10);
        await dbContext.Videos.AddRangeAsync(exampleVideosList);
        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var dbVideo = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        dbVideo.Should().NotBeNull();
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetWithAllProperties()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideoWithAllProperties();

        var categories = _fixture.GetExampleCategoriesList(3);
        await dbContext.Categories.AddRangeAsync(categories);
        foreach (var category in categories)
        {
            exampleVideo.AddCategory(category.Id);
            await dbContext.VideosCategories.AddAsync(
                new VideosCategories(exampleVideo.Id, category.Id)
            );
        }

        var genres = _fixture.GetExampleGenresList(3);
        await dbContext.Genres.AddRangeAsync(genres);
        foreach (var genre in genres)
        {
            exampleVideo.AddGenre(genre.Id);
            await dbContext.VideosGenres.AddAsync(
                new VideosGenres(exampleVideo.Id, genre.Id)
            );
        }

        var castMembers = _fixture.GetExampleCastMembersList(3);
        await dbContext.CastMembers.AddRangeAsync(castMembers);
        foreach (var castMember in castMembers)
        {
            exampleVideo.AddCastMember(castMember.Id);
            await dbContext.VideosCastMembers.AddAsync(
                new VideosCastMembers(exampleVideo.Id, castMember.Id)
            );
        }

        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var dbVideo = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        dbVideo.Should().NotBeNull();
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.CreatedAt.Should().Be(exampleVideo.CreatedAt);

        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);

        dbVideo.Media.Should().NotBeNull();
        dbValueMediaCheck(dbVideo.Media!, exampleVideo.Media!);

        dbVideo.Trailer.Should().NotBeNull();
        dbValueMediaCheck(dbVideo.Trailer!, exampleVideo.Trailer!);

        dbVideo.Categories.Should().HaveCount(categories.Count);
        dbVideo.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
        dbVideo.Genres.Should().HaveCount(genres.Count);
        dbVideo.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
        dbVideo.CastMembers.Should().HaveCount(castMembers.Count);
        dbVideo.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        await dbContext.Videos.AddRangeAsync(_fixture.GetExampleVideosListWithoutRelations(10));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(dbContext);
        var randomGuid = Guid.NewGuid();

        var action = async () => await videoRepository.Get(randomGuid, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Video with ID {randomGuid} not found.");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        var updatedValues = _fixture.GetExampleVideo();
        exampleVideo.Update(
            updatedValues.Title,
            updatedValues.Description,
            updatedValues.Opened,
            updatedValues.Published,
            updatedValues.YearLaunched,
            updatedValues.Duration,
            updatedValues.Rating
        );

        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(updatedValues.Title);
        dbVideo.Description.Should().Be(updatedValues.Description);
        dbVideo.Opened.Should().Be(updatedValues.Opened);
        dbVideo.Published.Should().Be(updatedValues.Published);
        dbVideo.YearLaunched.Should().Be(updatedValues.YearLaunched);
        dbVideo.Duration.Should().Be(updatedValues.Duration);
        dbVideo.Rating.Should().Be(updatedValues.Rating);
        dbVideo.CreatedAt.Should().Be(exampleVideo.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateImages))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateImages()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        var newThumbPath = _fixture.GetValidImagePath();
        var newThumbHalfPath = _fixture.GetValidImagePath();
        var newBannerPath = _fixture.GetValidImagePath();

        exampleVideo.UpdateThumb(newThumbPath);
        exampleVideo.UpdateThumbHalf(newThumbHalfPath);
        exampleVideo.UpdateBanner(newBannerPath);

        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(newThumbPath);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(newThumbHalfPath);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(newBannerPath);
    }

    [Fact(DisplayName = nameof(UpdateMediasAndRemoveOrphans))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateMediasAndRemoveOrphans()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideoWithAllProperties();
        var oldMediaId = exampleVideo.Media!.Id;
        var oldTrailerId = exampleVideo.Trailer!.Id;
        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        var newMediaPath = _fixture.GetValidMediaPath();
        var newEncodedPath = _fixture.GetValidMediaPath();
        var newTrailerPath = _fixture.GetValidMediaPath();

        exampleVideo.UpdateMedia(newMediaPath);
        exampleVideo.UpdateAsEncoded(newEncodedPath);
        exampleVideo.UpdateTrailer(newTrailerPath);

        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos
            .AsNoTracking()
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Media.Should().NotBeNull();
        dbVideo.Media!.Id.Should().Be(exampleVideo.Media!.Id);
        dbVideo.Media.FilePath.Should().Be(newMediaPath);
        dbVideo.Media.EncodedPath.Should().Be(newEncodedPath);
        dbVideo.Media.Status.Should().Be(EMediaStatus.Completed);

        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.Id.Should().Be(exampleVideo.Trailer!.Id);
        dbVideo.Trailer.FilePath.Should().Be(newTrailerPath);

        var savedMedias = await assertsDbContext.Set<Media>().ToListAsync();
        savedMedias.Should().HaveCount(2);
        savedMedias.Select(x => x.Id).Should().NotContain(oldMediaId);
        savedMedias.Select(x => x.Id).Should().NotContain(oldTrailerId);
    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();

        var initialCategories = _fixture.GetExampleCategoriesList(3);
        var newCategories = _fixture.GetExampleCategoriesList(2);
        await dbContext.Categories.AddRangeAsync(initialCategories);
        await dbContext.Categories.AddRangeAsync(newCategories);
        foreach (var category in initialCategories)
        {
            exampleVideo.AddCategory(category.Id);
            await dbContext.VideosCategories.AddAsync(
                new VideosCategories(exampleVideo.Id, category.Id)
            );
        }

        var initialGenres = _fixture.GetExampleGenresList(3);
        var newGenres = _fixture.GetExampleGenresList(2);
        await dbContext.Genres.AddRangeAsync(initialGenres);
        await dbContext.Genres.AddRangeAsync(newGenres);
        foreach (var genre in initialGenres)
        {
            exampleVideo.AddGenre(genre.Id);
            await dbContext.VideosGenres.AddAsync(
                new VideosGenres(exampleVideo.Id, genre.Id)
            );
        }

        var initialCastMembers = _fixture.GetExampleCastMembersList(3);
        var newCastMembers = _fixture.GetExampleCastMembersList(2);
        await dbContext.CastMembers.AddRangeAsync(initialCastMembers);
        await dbContext.CastMembers.AddRangeAsync(newCastMembers);
        foreach (var castMember in initialCastMembers)
        {
            exampleVideo.AddCastMember(castMember.Id);
            await dbContext.VideosCastMembers.AddAsync(
                new VideosCastMembers(exampleVideo.Id, castMember.Id)
            );
        }

        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        exampleVideo.RemoveAllCategories();
        newCategories.ForEach(c => exampleVideo.AddCategory(c.Id));

        exampleVideo.RemoveAllGenres();
        newGenres.ForEach(g => exampleVideo.AddGenre(g.Id));

        exampleVideo.RemoveAllCastMembers();
        newCastMembers.ForEach(cm => exampleVideo.AddCastMember(cm.Id));

        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);

        var dbVideosCategories = await assertsDbContext.VideosCategories
            .Where(x => x.VideoId == exampleVideo.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        dbVideosCategories.Should().HaveCount(newCategories.Count);
        dbVideosCategories.Should().BeEquivalentTo(newCategories.Select(x => x.Id));

        var dbVideosGenres = await assertsDbContext.VideosGenres
            .Where(x => x.VideoId == exampleVideo.Id)
            .Select(x => x.GenreId)
            .ToListAsync();
        dbVideosGenres.Should().HaveCount(newGenres.Count);
        dbVideosGenres.Should().BeEquivalentTo(newGenres.Select(x => x.Id));

        var dbVideosCastMembers = await assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == exampleVideo.Id)
            .Select(x => x.CastMemberId)
            .ToListAsync();
        dbVideosCastMembers.Should().HaveCount(newCastMembers.Count);
        dbVideosCastMembers.Should().BeEquivalentTo(newCastMembers.Select(x => x.Id));
    }

    [Fact(DisplayName = nameof(UpdateRemovingAllRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateRemovingAllRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();

        var initialCategories = _fixture.GetExampleCategoriesList(3);
        await dbContext.Categories.AddRangeAsync(initialCategories);
        foreach (var category in initialCategories)
        {
            exampleVideo.AddCategory(category.Id);
            await dbContext.VideosCategories.AddAsync(
                new VideosCategories(exampleVideo.Id, category.Id)
            );
        }

        var initialGenres = _fixture.GetExampleGenresList(3);
        await dbContext.Genres.AddRangeAsync(initialGenres);
        foreach (var genre in initialGenres)
        {
            exampleVideo.AddGenre(genre.Id);
            await dbContext.VideosGenres.AddAsync(
                new VideosGenres(exampleVideo.Id, genre.Id)
            );
        }

        var initialCastMembers = _fixture.GetExampleCastMembersList(3);
        await dbContext.CastMembers.AddRangeAsync(initialCastMembers);
        foreach (var castMember in initialCastMembers)
        {
            exampleVideo.AddCastMember(castMember.Id);
            await dbContext.VideosCastMembers.AddAsync(
                new VideosCastMembers(exampleVideo.Id, castMember.Id)
            );
        }

        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        exampleVideo.RemoveAllCategories();
        exampleVideo.RemoveAllGenres();
        exampleVideo.RemoveAllCastMembers();

        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);

        var dbVideosCategories = await assertsDbContext.VideosCategories
            .Where(x => x.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbVideosCategories.Should().BeEmpty();

        var dbVideosGenres = await assertsDbContext.VideosGenres
            .Where(x => x.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbVideosGenres.Should().BeEmpty();

        var dbVideosCastMembers = await assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbVideosCastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideoWithAllProperties();

        var categories = _fixture.GetExampleCategoriesList(2);
        await dbContext.Categories.AddRangeAsync(categories);
        foreach (var category in categories)
        {
            exampleVideo.AddCategory(category.Id);
            await dbContext.VideosCategories.AddAsync(
                new VideosCategories(exampleVideo.Id, category.Id)
            );
        }

        var genres = _fixture.GetExampleGenresList(2);
        await dbContext.Genres.AddRangeAsync(genres);
        foreach (var genre in genres)
        {
            exampleVideo.AddGenre(genre.Id);
            await dbContext.VideosGenres.AddAsync(
                new VideosGenres(exampleVideo.Id, genre.Id)
            );
        }

        var castMembers = _fixture.GetExampleCastMembersList(2);
        await dbContext.CastMembers.AddRangeAsync(castMembers);
        foreach (var castMember in castMembers)
        {
            exampleVideo.AddCastMember(castMember.Id);
            await dbContext.VideosCastMembers.AddAsync(
                new VideosCastMembers(exampleVideo.Id, castMember.Id)
            );
        }

        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(exampleVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbVideo = await assertsDbContext.Videos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);
        dbVideo.Should().BeNull();

        var dbVideosCategories = await assertsDbContext.VideosCategories
            .Where(x => x.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbVideosCategories.Should().BeEmpty();

        var dbVideosGenres = await assertsDbContext.VideosGenres
            .Where(x => x.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbVideosGenres.Should().BeEmpty();

        var dbVideosCastMembers = await assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbVideosCastMembers.Should().BeEmpty();

        var dbMedias = await assertsDbContext.Set<Media>().ToListAsync();
        dbMedias.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(SearchReturnListAndTotal))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetExampleVideosListWithoutRelations(15);
        await dbContext.Videos.AddRangeAsync(exampleVideosList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            AssertVideoProperties(outputItem, exampleItem!);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsRelationsAndMedias))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnsRelationsAndMedias()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetExampleVideosList(10);
        await dbContext.Videos.AddRangeAsync(exampleVideosList);

        foreach (var video in exampleVideosList)
        {
            var categories = _fixture.GetExampleCategoriesList(2);
            await dbContext.Categories.AddRangeAsync(categories);
            foreach (var category in categories)
            {
                video.AddCategory(category.Id);
                await dbContext.VideosCategories.AddAsync(
                    new VideosCategories(video.Id, category.Id)
                );
            }

            var genres = _fixture.GetExampleGenresList(2);
            await dbContext.Genres.AddRangeAsync(genres);
            foreach (var genre in genres)
            {
                video.AddGenre(genre.Id);
                await dbContext.VideosGenres.AddAsync(
                    new VideosGenres(video.Id, genre.Id)
                );
            }

            var castMembers = _fixture.GetExampleCastMembersList(2);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            foreach (var castMember in castMembers)
            {
                video.AddCastMember(castMember.Id);
                await dbContext.VideosCastMembers.AddAsync(
                    new VideosCastMembers(video.Id, castMember.Id)
                );
            }
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            AssertVideoProperties(outputItem, exampleItem!);

            outputItem.Thumb.Should().NotBeNull();
            outputItem.Thumb!.Path.Should().Be(exampleItem.Thumb!.Path);
            outputItem.ThumbHalf.Should().NotBeNull();
            outputItem.ThumbHalf!.Path.Should().Be(exampleItem.ThumbHalf!.Path);
            outputItem.Banner.Should().NotBeNull();
            outputItem.Banner!.Path.Should().Be(exampleItem.Banner!.Path);

            outputItem.Media.Should().NotBeNull();
            dbValueMediaCheck(outputItem.Media!, exampleItem.Media!);

            outputItem.Trailer.Should().NotBeNull();
            dbValueMediaCheck(outputItem.Trailer!, exampleItem.Trailer!);

            outputItem.Categories.Should().HaveCount(exampleItem.Categories.Count);
            outputItem.Categories.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Genres.Should().HaveCount(exampleItem.Genres.Count);
            outputItem.Genres.Should().BeEquivalentTo(exampleItem.Genres);
            outputItem.CastMembers.Should().HaveCount(exampleItem.CastMembers.Count);
            outputItem.CastMembers.Should().BeEquivalentTo(exampleItem.CastMembers);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var videoRepository = new Repository.VideoRepository(dbContext);

        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().BeEmpty();
    }

    [Theory(DisplayName = nameof(SearchWithPagination))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchWithPagination(
        int quantityVideosToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetExampleVideosListWithoutRelations(quantityVideosToGenerate);
        await dbContext.Videos.AddRangeAsync(exampleVideosList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(page, perPage, "", "", ESearchOrder.Asc);
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.Should().OnlyHaveUniqueItems();

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            AssertVideoProperties(outputItem, exampleItem!);
        }
    }

    [Theory(DisplayName = nameof(SearchByTitle))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData("Action", ESearchOrder.Asc, 1, 5, 3, 3)]
    [InlineData("Horror", ESearchOrder.Desc, 1, 5, 2, 2)]
    [InlineData("Horror", ESearchOrder.Desc, 2, 1, 1, 2)]
    [InlineData("Horror", ESearchOrder.Desc, 3, 1, 0, 2)]
    [InlineData("Drama", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Sci-fi", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("NotFound", ESearchOrder.Asc, 1, 5, 0, 0)]
    public async Task SearchByTitle(
        string searchTerm,
        ESearchOrder sortOrder,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetExampleVideosListByTitles(
            new List<string>
            {
                "Action",
                "Horror",
                "Horror - Action",
                "Drama",
                "Action Comedy",
                "Comedy",
                "Sci-fi",
                "Romance"
            }
        );
        await dbContext.Videos.AddRangeAsync(exampleVideosList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(page, perPage, searchTerm, "", sortOrder);
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.Should().OnlyHaveUniqueItems();

        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            AssertVideoProperties(outputItem, exampleItem!);
        }
    }

    [Theory(DisplayName = nameof(SearchWithOrderByAndOrder))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData("title", ESearchOrder.Asc)]
    [InlineData("title", ESearchOrder.Desc)]
    [InlineData("id", ESearchOrder.Asc)]
    [InlineData("id", ESearchOrder.Desc)]
    [InlineData("createdAt", ESearchOrder.Asc)]
    [InlineData("createdAt", ESearchOrder.Desc)]
    [InlineData("", ESearchOrder.Asc)]
    [InlineData("", ESearchOrder.Desc)]
    public async Task SearchWithOrderByAndOrder(
        string orderBy,
        ESearchOrder searchOrder
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetExampleVideosListByTitles(
            new List<string>
            {
                "Action",
                "Horror",
                "Horror - Action",
                "Drama",
                "Action Comedy",
                "Comedy"
            }
        );
        await dbContext.Videos.AddRangeAsync(exampleVideosList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var videoRepository = new Repository.VideoRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(1, 10, "", orderBy, searchOrder);
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        var expectedOrderedList = (orderBy.ToLower(), searchOrder) switch
        {
            ("title", ESearchOrder.Asc) => exampleVideosList
                .OrderBy(x => x.Title)
                .ThenBy(x => x.Id)
                .ToList(),
            ("title", ESearchOrder.Desc) => exampleVideosList
                .OrderByDescending(x => x.Title)
                .ThenBy(x => x.Id)
                .ToList(),
            ("id", ESearchOrder.Asc) => exampleVideosList
                .OrderBy(x => x.Id)
                .ToList(),
            ("id", ESearchOrder.Desc) => exampleVideosList
                .OrderByDescending(x => x.Id)
                .ToList(),
            ("createdat", ESearchOrder.Asc) => exampleVideosList
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
            ("createdat", ESearchOrder.Desc) => exampleVideosList
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
            (_, ESearchOrder.Asc) => exampleVideosList
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
            _ => exampleVideosList
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
        };

        for (int i = 0; i < output.Items.Count; i++)
        {
            output.Items[i].Id.Should().Be(expectedOrderedList[i].Id);
            AssertVideoProperties(output.Items[i], expectedOrderedList[i]);
        }
    }

    private static void AssertVideoProperties(Video actual, Video expected)
    {
        actual.Id.Should().Be(expected.Id);
        actual.Title.Should().Be(expected.Title);
        actual.Description.Should().Be(expected.Description);
        actual.Opened.Should().Be(expected.Opened);
        actual.Published.Should().Be(expected.Published);
        actual.YearLaunched.Should().Be(expected.YearLaunched);
        actual.Duration.Should().Be(expected.Duration);
        actual.Rating.Should().Be(expected.Rating);
        actual.CreatedAt.Should().Be(expected.CreatedAt);
    }

    private static void dbValueMediaCheck(Media actual, Media expected)
    {
        actual.Id.Should().Be(expected.Id);
        actual.FilePath.Should().Be(expected.FilePath);
        actual.EncodedPath.Should().Be(expected.EncodedPath);
        actual.Status.Should().Be(expected.Status);
    }
}
