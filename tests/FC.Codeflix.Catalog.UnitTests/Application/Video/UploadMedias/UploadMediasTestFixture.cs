using FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[CollectionDefinition(nameof(UploadMediasTestFixture))]
public record UploadMediaTestFixtureCollection : ICollectionFixture<UploadMediasTestFixture>;

public class UploadMediasTestFixture : VideoTestFixtureBase
{
    public UploadMediasInput GetValidInput(Guid? videoId = null)
        => new(
            VideoId: videoId ?? Guid.NewGuid(),
            VideoFile: GetValidImageFileInput(),
            TrailerFile: GetValidImageFileInput()
        );
}
