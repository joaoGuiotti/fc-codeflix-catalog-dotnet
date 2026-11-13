using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.DeleteVideo;

[Collection(nameof(VideoApiBaseFixture))]
public class DeleteVideoApiTest : IDisposable
{
    private readonly VideoApiBaseFixture _fixture;

    public DeleteVideoApiTest(VideoApiBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("EndToEnd/API", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideo()
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

        var mediaCountBefore = await _fixture.VideoPersistence.GetMediaCountAsync();

        var expectedMediaCount = mediaCountBefore - 2;
        var targetVideo = exampleVideos.ElementAt(5);

        var allTargetVideoMedias = new[]
        {
            targetVideo.Trailer!.FilePath,
            targetVideo.Media!.FilePath,
            targetVideo.Banner!.Path,
            targetVideo.Thumb!.Path,
            targetVideo.ThumbHalf!.Path,
        };

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/videos/{targetVideo.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var videoFromDb = await _fixture.VideoPersistence.GetById(targetVideo.Id);
        videoFromDb.Should().BeNull();

        var categoriesFromDb = await _fixture.VideoPersistence
            .GetVideosCategoriesAsync(targetVideo.Id);
        categoriesFromDb.Should().BeEmpty();

        var genresFromDb = await _fixture.VideoPersistence
            .GetVideosGenresAsync(targetVideo.Id);
        genresFromDb.Should().BeEmpty();

        var castMembersFromDb = await _fixture.VideoPersistence
            .GetVideosCastMembersAsync(targetVideo.Id);
        castMembersFromDb.Should().BeEmpty();

        var actualMediaCount = await _fixture.VideoPersistence.GetMediaCountAsync();
        actualMediaCount.Should().Be(expectedMediaCount);

        _fixture.WebAppFactory.StorageClient!.Verify(
           x => x.DeleteObjectAsync(
               It.IsAny<string>(),
               It.Is<string>(fileName => allTargetVideoMedias.Contains(fileName)),
               It.IsAny<DeleteObjectOptions>(),
               It.IsAny<CancellationToken>()),
           Times.Exactly(5));
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.DeleteObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/API", "Video/DeleteVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoColletion(2);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var targetVideoId = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/videos/{targetVideoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video with ID {targetVideoId} not found.");
    }

    public void Dispose()
        => _fixture.DisposePersistence();
}

