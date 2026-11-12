using FC.Codeflix.Catalog.Api.Models.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.CreateVideo;

[Collection(nameof(VideoApiBaseFixture))]
public class CreateVideoApiTest
    : IDisposable
{
    private readonly VideoApiBaseFixture _fixture;

    public CreateVideoApiTest(VideoApiBaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateBasicVideo))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateBasicVideo()
    {
        var input = _fixture.GetBasicCreateVideoInput();
        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.Title.Should().Be(input.Title);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating);
    }

    [Fact(DisplayName = nameof(CreateVideoWithRelations))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithRelations()
    {
        var categories = _fixture.GetExampleCategoriesList(5);
        await _fixture.CategoryPersistence.BulkInsert(categories);

        var genres = _fixture.GetExampleGenreList(5);
        await _fixture.GenrePersistence.BulkInsert(genres);

        var castMembers = _fixture.GetExampleCastMembersList(5);
        await _fixture.CastMemberPersistence.BulkInsert(castMembers);

        var input = _fixture.GetBasicCreateVideoInputWithRelations(categories, genres, castMembers);

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.Title.Should().Be(input.Title);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var outputCategoryIds = output.Data.Categories.Select(c => c.Id).ToList();
        outputCategoryIds.Should().NotBeEmpty();
        outputCategoryIds.Should().BeEquivalentTo(input.CategoriesIds);
        var outputGenreIds = output.Data.Genres.Select(c => c.Id).ToList();
        outputGenreIds.Should().NotBeEmpty();
        outputGenreIds.Should().BeEquivalentTo(input.GenresIds);
        var outputCastMemberIds = output.Data.CastMembers.Select(c => c.Id).ToList();
        outputCastMemberIds.Should().NotBeEmpty();
        outputCastMemberIds.Should().BeEquivalentTo(input.CastMembersIds);
        var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!);
        var categoriesFromDb = await _fixture.VideoPersistence
            .GetVideosCategoriesAsync(videoFromDb.Id);
        categoriesFromDb.Should().NotBeNull();
        var categoriesIdsFromDb = categoriesFromDb.Select(x => x.CategoryId);
        categoriesIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
        var genresFromDb = await _fixture.VideoPersistence
            .GetVideosGenresAsync(videoFromDb.Id);
        genresFromDb.Should().NotBeNull();
        var genresIdsFromDb = genresFromDb.Select(x => x.GenreId);
        genresIdsFromDb.Should().BeEquivalentTo(input.GenresIds);
        var castMembersFromDb = await _fixture.VideoPersistence
            .GetVideosCastMembersAsync(videoFromDb.Id);
        castMembersFromDb.Should().NotBeNull();
        var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId);
        castMembersIdsFromDb.Should().BeEquivalentTo(input.CastMembersIds);
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidGenreId))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidGenreId()
    {
        var invalidGenreId = Guid.NewGuid();
        var input = _fixture.GetBasicCreateVideoInput() with
        {
            GenresIds = [invalidGenreId]
        };

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related Genre id (or ids) not found: {invalidGenreId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCategory))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCategory()
    {
        var invalidCategoryId = Guid.NewGuid();
        var input = _fixture.GetBasicCreateVideoInput() with
        {
            CategoriesIds = [invalidCategoryId]
        };

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);


        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related Category id (or ids) not found: {invalidCategoryId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCastMember))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCastMember()
    {
        var invalidCastMemberId = Guid.NewGuid();
        var input = _fixture.GetBasicCreateVideoInput() with
        {
            CastMembersIds = [invalidCastMemberId]
        };

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related CastMember id (or ids) not found: {invalidCastMemberId}.");
    }

    public void Dispose()
     => _fixture.DisposePersistence();
}
