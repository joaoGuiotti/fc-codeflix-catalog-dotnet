using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>
{
}

public class VideoTestFixture : BaseFixture
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
}
