using System.Net;
using FC.Codeflix.Catalog.Api.Models.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.GetVideo;

[Collection(nameof(VideoApiBaseFixture))]
public class GetVideoApiTest : IDisposable
{
    private readonly VideoApiBaseFixture _fixture;

    public GetVideoApiTest(VideoApiBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetVideo))]
    [Trait("EndToEnd/API", "Video/GetVideo - Endpoints")]
    public async Task GetVideo()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        var exampleGenres = _fixture.GetExampleGenreList(4);
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

        var exampleVideo = exampleVideos.ElementAt(5);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<VideoModelOutput>>($"/videos/{exampleVideo.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleVideo.Id);
        output.Data.Title.Should().Be(exampleVideo.Title);
        output.Data.Description.Should().Be(exampleVideo.Description);
        output.Data.Opened.Should().Be(exampleVideo.Opened);
        output.Data.Published.Should().Be(exampleVideo.Published);
        output.Data.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Data.Duration.Should().Be(exampleVideo.Duration);
        output.Data.Rating.Should().Be(exampleVideo.Rating);
        output.Data.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
        output.Data.Thumb.Should().Be(exampleVideo.Thumb!.Path);
        output.Data.ThumbHalf.Should().Be(exampleVideo.ThumbHalf!.Path);
        output.Data.Banner.Should().Be(exampleVideo.Banner!.Path);
        output.Data.Media.Should().Be(exampleVideo.Media!.FilePath);
        output.Data.Trailer.Should().Be(exampleVideo.Trailer!.FilePath);

        var expectedCategories = exampleCategories
            .Select(category => new VideoModelOutputRelatedAggregate(category.Id, category.Name));
        output.Data.Categories.Should().BeEquivalentTo(expectedCategories);

        var expectedGenres = exampleGenres
            .Select(genre => new VideoModelOutputRelatedAggregate(genre.Id, genre.Name));
        output.Data.Genres.Should().BeEquivalentTo(expectedGenres);

        var expectedCastMembers = exampleCastMembers
            .Select(castMember => new VideoModelOutputRelatedAggregate(castMember.Id, null));
        output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
    }

    [Fact(DisplayName = nameof(Error404WhenIdNotFound))]
    [Trait("EndToEnd/API", "Video/GetVideo - Endpoints")]
    public async Task Error404WhenIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(10);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient
            .Get<ProblemDetails>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video with ID {videoId} not found.");
    }

    public void Dispose()
        => _fixture.DisposePersistence();
}

