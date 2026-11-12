using FC.Codeflix.Catalog.Api.Models.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.ListVideo;

[Collection(nameof(VideoApiBaseFixture))]
public class ListVideoApiTest : IDisposable
{
    public readonly VideoApiBaseFixture _fixture;

    public ListVideoApiTest(VideoApiBaseFixture fixture)
        => _fixture = fixture;

    public void Dispose()
        => _fixture.DisposePersistence();

    [Fact(DisplayName = nameof(ListVideos))]
    [Trait("EndToEnd/API", "Video/ListVideo - Endpoints")]
    public async Task ListVideos()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        var exampleGenres = _fixture.GetExampleGenreList(4);
        var exampleCastMembers = _fixture.GetExampleCastMembersList(5);

        var exampleVideos = _fixture.GetVideoColletion(10);

        exampleVideos.ForEach(video =>
        {
            exampleCategories.ForEach(category => video.AddCategory(category.Id));
            exampleCastMembers.ForEach(castMember => video.AddCastMember(castMember.Id));
            exampleGenres.ForEach(genre => video.AddGenre(genre.Id));
        });

        await _fixture.CategoryPersistence.BulkInsert(exampleCategories);
        await _fixture.CastMemberPersistence.BulkInsert(exampleCastMembers);
        await _fixture.GenrePersistence.BulkInsert(exampleGenres);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var input = new ListRequest
        {
            Page = 1,
            PerPage = exampleVideos.Count
        };

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleVideos.Count);
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(exampleVideos.Count);
        output.Data.ToList().ForEach(outputVideo =>
        {
            var exampleVideo = exampleVideos.FirstOrDefault(x => x.Id == outputVideo.Id);
            exampleVideo.Should().NotBeNull();
            outputVideo.Id.Should().Be(exampleVideo!.Id);
            outputVideo.Title.Should().Be(exampleVideo.Title);
            outputVideo.Description.Should().Be(exampleVideo.Description);
            outputVideo.Duration.Should().Be(exampleVideo.Duration);
            outputVideo.Rating.Should().Be(exampleVideo.Rating);
            outputVideo.Published.Should().Be(exampleVideo.Published);
            outputVideo.Opened.Should().Be(exampleVideo.Opened);
            outputVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputVideo.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
            outputVideo.Thumb.Should().Be(exampleVideo.Thumb!.Path);
            outputVideo.ThumbHalf.Should().Be(exampleVideo.ThumbHalf!.Path);
            outputVideo.Banner.Should().Be(exampleVideo.Banner!.Path);
            outputVideo.Media.Should().Be(exampleVideo.Media!.FilePath);
            outputVideo.Trailer.Should().Be(exampleVideo.Trailer!.FilePath);

            var expectedCategories = exampleCategories
                .Select(category => new VideoModelOutputRelatedAggregate(category.Id, category.Name));
            outputVideo.Categories.Should().BeEquivalentTo(expectedCategories);

            var expectedGenres = exampleGenres
                .Select(genre => new VideoModelOutputRelatedAggregate(genre.Id, genre.Name));
            outputVideo.Genres.Should().BeEquivalentTo(expectedGenres);

            var expectedCastMembers = exampleCastMembers
                .Select(castMember => new VideoModelOutputRelatedAggregate(castMember.Id, castMember.Name));
            outputVideo.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
        });
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Video/ListVideo - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<VideoModelOutput>>("/videos");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(15);
        output.Meta.Total.Should().Be(0);
        output.Data.Should().NotBeNull();
        output.Data.Should().BeEmpty();
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Video/ListVideo - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityVideosToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleVideos = _fixture.GetVideoColletion(quantityVideosToGenerate);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var input = new ListRequest
        {
            Page = page,
            PerPage = perPage
        };

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleVideos.Count);
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Data.ToList().ForEach(outputVideo =>
        {
            var exampleVideo = exampleVideos.FirstOrDefault(x => x.Id == outputVideo.Id);
            exampleVideo.Should().NotBeNull();
            outputVideo.Id.Should().Be(exampleVideo!.Id);
            outputVideo.Title.Should().Be(exampleVideo.Title);
            outputVideo.Description.Should().Be(exampleVideo.Description);
            outputVideo.Duration.Should().Be(exampleVideo.Duration);
            outputVideo.Rating.Should().Be(exampleVideo.Rating);
            outputVideo.Published.Should().Be(exampleVideo.Published);
            outputVideo.Opened.Should().Be(exampleVideo.Opened);
            outputVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputVideo.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Video/ListVideo - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var videoTitlesList = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        };
        var exampleVideos = _fixture.GetVideoColletion(videoTitlesList);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var input = new ListRequest
        {
            Page = page,
            PerPage = perPage,
            Search = search
        };

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Data.ToList().ForEach(outputVideo =>
        {
            var exampleVideo = exampleVideos.FirstOrDefault(x => x.Id == outputVideo.Id);
            exampleVideo.Should().NotBeNull();
            outputVideo.Id.Should().Be(exampleVideo!.Id);
            outputVideo.Title.Should().Be(exampleVideo.Title);
            outputVideo.Description.Should().Be(exampleVideo.Description);
            outputVideo.Duration.Should().Be(exampleVideo.Duration);
            outputVideo.Rating.Should().Be(exampleVideo.Rating);
            outputVideo.Published.Should().Be(exampleVideo.Published);
            outputVideo.Opened.Should().Be(exampleVideo.Opened);
            outputVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputVideo.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
        });
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API", "Video/ListVideo - Endpoints")]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    [InlineData("", "desc")]
    public async Task ListOrdered(
        string orderBy,
        string order
    )
    {
        var exampleVideos = _fixture.GetVideoColletion(10);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var inputOrder = order == "asc" ? ESearchOrder.Asc : ESearchOrder.Desc;
        var input = new ListRequest
        {
            Page = 1,
            PerPage = exampleVideos.Count,
            Search = "",
            Sort = orderBy,
            SortDir = inputOrder
        };

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleVideos.Count);
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(exampleVideos.Count);

        var expectedOrderedList = _fixture.CloneVideosListOrdered(
            exampleVideos,
            input.Sort ?? "",
            input.SortDir
        );

        for (int index = 0; index < expectedOrderedList.Count; index++)
        {
            var outputVideo = output.Data[index];
            var exampleVideo = expectedOrderedList[index];
            outputVideo.Should().NotBeNull();
            outputVideo.Id.Should().Be(exampleVideo.Id);
            outputVideo.Title.Should().Be(exampleVideo.Title);
            outputVideo.Description.Should().Be(exampleVideo.Description);
            outputVideo.Duration.Should().Be(exampleVideo.Duration);
            outputVideo.Rating.Should().Be(exampleVideo.Rating);
            outputVideo.Published.Should().Be(exampleVideo.Published);
            outputVideo.Opened.Should().Be(exampleVideo.Opened);
            outputVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputVideo.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
        }
    }
}
