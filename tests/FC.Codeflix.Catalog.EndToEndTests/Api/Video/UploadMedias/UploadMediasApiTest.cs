using System.Net;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.UploadMedias;

[Collection(nameof(VideoApiBaseFixture))]
public class UploadMediasApiTest : IDisposable
{
    private readonly VideoApiBaseFixture _fixture;

    public UploadMediasApiTest(VideoApiBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UploadBanner))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task UploadBanner()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = exampleVideos.ElementAt(2).Id;
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(
            videoId,
            nameof(Domain.Entity.Video.Banner),
            file.Extension
        );

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Banner.Should().NotBeNull();
        videoFromDb.Banner!.Path.Should().Be(expectedFileName);
    }

    [Fact(DisplayName = nameof(UploadThumb))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task UploadThumb()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = exampleVideos.ElementAt(2).Id;
        var mediaType = "thumbnail";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(
            videoId,
            nameof(Domain.Entity.Video.Thumb),
            file.Extension
        );

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Thumb.Should().NotBeNull();
        videoFromDb.Thumb!.Path.Should().Be(expectedFileName);
    }

    [Fact(DisplayName = nameof(UploadThumbHalf))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task UploadThumbHalf()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = exampleVideos.ElementAt(2).Id;
        var mediaType = "thumbnail_half";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(
            videoId,
            nameof(Domain.Entity.Video.ThumbHalf),
            file.Extension
        );

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.ThumbHalf.Should().NotBeNull();
        videoFromDb.ThumbHalf!.Path.Should().Be(expectedFileName);
    }

    [Fact(DisplayName = nameof(UploadTrailer))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task UploadTrailer()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = exampleVideos.ElementAt(2).Id;
        var mediaType = "trailer";
        var file = _fixture.GetValidMediaFileInput();
        var expectedFileName = StorageFileName.Create(
            videoId,
            nameof(Domain.Entity.Video.Trailer),
            file.Extension
        );

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Trailer.Should().NotBeNull();
        videoFromDb.Trailer!.FilePath.Should().Be(expectedFileName);
    }

    [Fact(DisplayName = nameof(UploadVideo))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task UploadVideo()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = exampleVideos.ElementAt(2).Id;
        var mediaType = "video";
        var file = _fixture.GetValidMediaFileInput();
        var expectedFileName = StorageFileName.Create(
            videoId,
            nameof(Domain.Entity.Video.Media),
            file.Extension
        );

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Media.Should().NotBeNull();
        videoFromDb.Media!.FilePath.Should().Be(expectedFileName);
    }

    [Fact(DisplayName = nameof(Error422WhenMediaTypeIsInvalid))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task Error422WhenMediaTypeIsInvalid()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = exampleVideos.ElementAt(2).Id;
        var mediaType = "invalid_type";
        var file = _fixture.GetValidMediaFileInput();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<ProblemDetails>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("UnprocessableEntity");
        output.Title.Should().Be("One or more validation errors occurred.");
        output.Detail.Should().Be($"'{mediaType}' is not a valid media type.");
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdIsNotFound))]
    [Trait("EndToEnd/API", "Video/UploadMedias - Endpoints")]
    public async Task Error404WhenVideoIdIsNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var videoId = Guid.NewGuid();
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<ProblemDetails>($"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Video with ID {videoId} not found.");
    }

    public void Dispose()
        => _fixture.DisposePersistence();
}

