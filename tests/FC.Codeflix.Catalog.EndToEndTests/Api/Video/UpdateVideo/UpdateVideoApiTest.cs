using System.Net;
using FC.Codeflix.Catalog.Api.Models.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.UpdateVideo;

[Collection(nameof(VideoApiBaseFixture))]
public class UpdateVideoApiTest : IDisposable
{
    private readonly VideoApiBaseFixture _fixture;

    public UpdateVideoApiTest(VideoApiBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateBasicVideo))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task UpdateBasicVideo()
    {
        var exampleVideos = _fixture.GetVideoColletion(10);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(5).Id;
        var input = _fixture.GetBasicUpdateVideoInput();

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<VideoModelOutput>>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetVideoId);
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);

        var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(targetVideoId);
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithRelations))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task UpdateVideoWithRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        var exampleGenres = _fixture.GetExampleGenreList(5);
        var exampleCastMembers = _fixture.GetExampleCastMembersList(5);
        var exampleVideos = _fixture.GetVideoColletion(10);

        exampleVideos.ForEach(video =>
        {
            exampleCategories.ForEach(category => video.AddCategory(category.Id));
            exampleGenres.ForEach(genre => video.AddGenre(genre.Id));
            exampleCastMembers.ForEach(castMember => video.AddCastMember(castMember.Id));
        });

        await _fixture.CategoryPersistence.BulkInsert(exampleCategories);
        await _fixture.GenrePersistence.BulkInsert(exampleGenres);
        await _fixture.CastMemberPersistence.BulkInsert(exampleCastMembers);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(5).Id;
        var targetCategories = exampleCategories.Take(3).ToList();
        var targetGenres = exampleGenres.Take(2).ToList();
        var targetCastMembers = exampleCastMembers.Take(4).ToList();

        var input = _fixture.GetUpdateVideoInputWithRelations(
            targetCategories,
            targetGenres,
            targetCastMembers
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<VideoModelOutput>>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetVideoId);
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);

        var outputCategoryIds = output.Data.Categories.Select(c => c.Id).ToList();
        outputCategoryIds.Should().NotBeEmpty();
        outputCategoryIds.Should().BeEquivalentTo(input.CategoriesId);

        var outputGenreIds = output.Data.Genres.Select(g => g.Id).ToList();
        outputGenreIds.Should().NotBeEmpty();
        outputGenreIds.Should().BeEquivalentTo(input.GenresId);

        var outputCastMemberIds = output.Data.CastMembers.Select(cm => cm.Id).ToList();
        outputCastMemberIds.Should().NotBeEmpty();
        outputCastMemberIds.Should().BeEquivalentTo(input.CastMembersId);

        var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(targetVideoId);
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
        categoriesIdsFromDb.Should().BeEquivalentTo(input.CategoriesId);

        var genresFromDb = await _fixture.VideoPersistence
            .GetVideosGenresAsync(videoFromDb.Id);
        genresFromDb.Should().NotBeNull();
        var genresIdsFromDb = genresFromDb.Select(x => x.GenreId);
        genresIdsFromDb.Should().BeEquivalentTo(input.GenresId);

        var castMembersFromDb = await _fixture.VideoPersistence
            .GetVideosCastMembersAsync(videoFromDb.Id);
        castMembersFromDb.Should().NotBeNull();
        var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId);
        castMembersIdsFromDb.Should().BeEquivalentTo(input.CastMembersId);
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(2);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = Guid.NewGuid();
        var input = _fixture.GetBasicUpdateVideoInput();

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video with ID {targetVideoId} not found.");
    }

    [Fact(DisplayName = nameof(Error422WhenCategoryIdNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error422WhenCategoryIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(2);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(1).Id;
        var invalidCategoryId = Guid.NewGuid();
        var input = _fixture.GetBasicUpdateVideoInput();
        input.CategoriesId = [invalidCategoryId];

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related Category id (or ids) not found: {invalidCategoryId}.");
    }

    [Fact(DisplayName = nameof(Error422WhenGenreIdNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error422WhenGenreIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(2);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(1).Id;
        var invalidGenreId = Guid.NewGuid();
        var input = _fixture.GetBasicUpdateVideoInput();
        input.GenresId = [invalidGenreId];

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related Genre id (or ids) not found: {invalidGenreId}.");
    }

    [Fact(DisplayName = nameof(Error422WhenCastMemberIdNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error422WhenCastMemberIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(2);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(1).Id;
        var invalidCastMemberId = Guid.NewGuid();
        var input = _fixture.GetBasicUpdateVideoInput();
        input.CastMembersId = [invalidCastMemberId];

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related CastMember id (or ids) not found: {invalidCastMemberId}.");
    }

    [Fact(DisplayName = nameof(Error422WhenEntityHasInvalidInput))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error422WhenEntityHasInvalidInput()
    {
        var exampleVideos = _fixture.GetVideoColletion(2);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(1).Id;
        var input = _fixture.GetBasicUpdateVideoInput();
        input.Title = _fixture.GetTooShortTitle();
        input.Description = _fixture.GetTooShortDescription();

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetVideoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("UnprocessableEntity");
        output.Title.Should().Be("One or more validation errors occurred.");
        output.Detail.Should().Be("There are validation errors in the entity");
    }

    public void Dispose()
        => _fixture.DisposePersistence();
}

