using System.Text;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>
{
}

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    public Mock<IVideoRepository> GetRepoMock()
        => new();

    public Mock<ICategoryRepository> GetCategoryRepoMock()
        => new();

    public Mock<IGenreRepository> GetGenreRepoMock()
        => new();

    public Mock<ICastMemberRepository> GetCastMemberRepoMock()
        => new();

    public Mock<IStorageService> GetStorageServiceMock()
        => new();

    public CreateVideoInput CreateVideoInput(
        string? title = null,
        string? description = null,
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null
    )
        => new(
            title ?? GetValidTitle(),
            description ?? GetValidDescription(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRating(),
            CategoriesIds: categoriesIds ?? [],
            GenresIds: genresIds ?? [],
            CastMembersIds: castMembersIds ?? [],
            Thumb: thumb,
            Banner: banner,
            ThumbHalf: thumbHalf
        );

    public FileInput GetValidImageFileInput()
        => new("jpg", new MemoryStream(Encoding.ASCII.GetBytes("Test")));
}
