using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Common;

public class StorageFileNameTest
{
    public static TheoryData<(Guid Id, string PropertyName, string Extension), string> GetStorageFileNameData()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        return new TheoryData<(Guid Id, string PropertyName, string Extension), string>
        {
            { (id1, " BannerPropertieName ", " .jpg "), $"{id1}-bannerpropertiename.jpg" },
            { (id2, "MeuVideoAAAA ", ".mp4 "), $"{id2}-meuvideoaaaa.mp4" },
            { (id3, "ASDASDSADAD ", "png "), $"{id3}-asdasdsadad.png" },
        };
    }

    [Theory(DisplayName = nameof(CreateStorageFileName))]
    [Trait("StorageFileName", "Create")]
    [MemberData(nameof(GetStorageFileNameData))]
    public void CreateStorageFileName(
        (Guid Id, string PropertyName, string Extension) input,
        string expectedFileName
    )
    {
        var fileName = StorageFileName.Create(input.Id, input.PropertyName, input.Extension);

        fileName.Should().NotBeNull();
        fileName.Should().Be(expectedFileName);
    }
}
