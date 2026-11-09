using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class MediaTest
{
    private readonly VideoTestFixture _fixture;

    public MediaTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Media - Entities")]
    public void Instantiate()   
    {
        var expectedFileMediaPath = _fixture.GetValidMediaPath();
        var media = new Media(expectedFileMediaPath);

        media.Id.Should().NotBeEmpty();
        media.FilePath.Should().Be(expectedFileMediaPath);
        media.Status.Should().Be(EMediaStatus.Pending);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncoded))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsSentToEncoded()
    {
        var media = _fixture.GetValidMedia();
        media.UpdateAsSentToEncode();

        media.Status.Should().Be(EMediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsEncoded()
    {
        var media = _fixture.GetValidMedia();
        media.UpdateAsSentToEncode();
        var encodedExamplePath = _fixture.GetValidMediaPath();
        media.UpdateAsEnconded(encodedExamplePath);

        media.Status.Should().Be(EMediaStatus.Completed);
        media.EncodedPath.Should().Be(encodedExamplePath);
    }
}
