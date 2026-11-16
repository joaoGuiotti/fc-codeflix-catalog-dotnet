using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Google.Cloud.Storage.V1;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateMediaStatus;

[CollectionDefinition(nameof(UpdateMeidaStatusTestFixture))]
public class UpdateMediaStatusTestFixtureCollextion
    : ICollectionFixture<UpdateMeidaStatusTestFixture>
{ }

public class UpdateMeidaStatusTestFixture
    : VideoTestFixtureBase
{
    public UpdateMediaStatusInput GetSuccededEncodingInput(Guid videoId)
        => new(
            videoId,
            EMediaStatus.Completed,
            EncodedPath: GetValidMediaPath()
        );

    public UpdateMediaStatusInput GetFailedEncodingInput(Guid videoId)
        => new(
            videoId,
            EMediaStatus.Error,
            ErrorMessage: "There was an error while trying to encode video."
        );
    public UpdateMediaStatusInput GetInvalidStatusInput(Guid videoId)
        => new(
            videoId,
            EMediaStatus.Processing,
            ErrorMessage: "There was an error while trying to encode video."
        );
}
