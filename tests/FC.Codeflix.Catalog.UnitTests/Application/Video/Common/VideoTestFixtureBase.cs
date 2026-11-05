using System.Text;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Common;

public class VideoTestFixtureBase : BaseFixture
{
    public DomainEntity.Video GetValidVideo(
        string? title = null,
        string? description = null,
        bool? opened = null,
        bool? published = null,
        int? yearLaunched = null,
        int? duration = null,
        ERating? rating = null,
        Image? thumb = null,
        Image? thumbHalf = null,
        Image? banner = null
    ) => new(
        title ?? GetValidTitle(),
        description ?? GetValidDescription(),
        opened ?? GetRandomBoolean(),
        published ?? GetRandomBoolean(),
        yearLaunched ?? GetValidYearLaunched(),
        duration ?? GetValidDuration(),
        rating ?? GetRandomRating(),
        thumb,
        thumbHalf,
        banner
    );

    public DomainEntity.Video GetValidVideoWithAllProperties()
    {
        var video = new DomainEntity.Video(
            GetValidTitle(),
            GetValidDescription(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRating()
        );

        video.UpdateBanner(GetValidImagePath());
        video.UpdateThumb(GetValidImagePath());
        video.UpdateThumbHalf(GetValidImagePath());

        video.UpdateMedia(GetValidMediaPath());
        video.UpdateTrailer(GetValidMediaPath());

        GetListGuids(Random.Shared.Next(2, 5)).ForEach(video.AddCategory);
        GetListGuids(Random.Shared.Next(2, 5)).ForEach(video.AddGenre);
        GetListGuids(Random.Shared.Next(2, 5)).ForEach(video.AddCastMember);

        return video;
    }

    public string GetValidImagePath()
        => Faker.Image.PlaceImgUrl();

    public string GetValidTitle()
    {
        var title = "";
        while (title.Length < 3)
            title = Faker.Commerce.ProductName();
        if (title.Length > 255)
            title = title[..255];
        return title;
    }

    public string GetValidDescription()
    {
        var description = "";
        while (description.Length < 3)
            description = Faker.Commerce.ProductDescription();
        if (description.Length > 255)
            description = description[..255];
        return description;
    }

    public bool GetRandomBoolean()
        => Faker.Random.Bool();

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(
            new DateOnly(1960, 1, 1),
            new DateOnly(2026, 12, 31)
        ).Year;

    public int GetValidDuration()
        => Faker.Random.Int(50, 300);

    public ERating GetRandomRating()
        => Faker.Random.Enum<ERating>();

    public string GetTooShortTitle()
        => Faker.Lorem.Letter(2);

    public string GetTooLongTitle()
        => Faker.Lorem.Letter(256);

    public string GetTooShortDescription()
        => Faker.Lorem.Letter(2);

    public string GetTooLongDescription()
        => Faker.Lorem.Letter(256);

    public string GetValidMediaPath()
    {
        var exampleMedias = new string[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/file.mp4",
            "https://www.s3.com.br/file-example.mp4",
            "https://www.globlstorage.com/file.mp4"
        };
        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }

    public DomainEntity.Media GetValidMedia()
        => new(GetValidMediaPath());

    public List<Guid> GetListGuids(int length = 12)
        => Enumerable.Range(1, length).Select(_ => Guid.NewGuid()).ToList();

    public FileInput GetValidImageFileInput()
        => new("jpg", new MemoryStream(Encoding.ASCII.GetBytes("Test")));

    public FileInput GetValidMediaFileInput()
        => new("mp4", new MemoryStream(Encoding.ASCII.GetBytes("Test")));
}
