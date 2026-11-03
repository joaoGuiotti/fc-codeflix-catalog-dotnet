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

    public CreateVideoInput CreateVideInput(string? title = null,  string? description = null)
        => new(
            title ?? GetValidTitle(),
            description ??  GetValidDescription(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRating()
        );
}
