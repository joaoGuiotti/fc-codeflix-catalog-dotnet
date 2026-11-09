using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.IntegrationTests.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection
    : ICollectionFixture<VideoRepositoryTestFixture> { }

public class VideoRepositoryTestFixture : BaseFixture
{
    public Video GetExampleVideo(string? title = null)
        => new(
            title ?? GetValidTitle(),
            GetValidDescription(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRating()
        );

    public Video GetExampleVideoWithAllProperties(string? title = null)
    {
        var video = new Video(
            title ?? GetValidTitle(),
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

        return video;
    }

    public List<Video> GetExampleVideosList(int count = 10)
        => Enumerable.Range(1, count).Select(_ => GetExampleVideoWithAllProperties()).ToList();

    public List<Video> GetExampleVideosListWithoutRelations(int count = 10)
        => Enumerable.Range(1, count).Select(_ => GetExampleVideo()).ToList();

    public List<Video> GetExampleVideosListByTitles(List<string> titles)
        => titles.Select(title => GetExampleVideo(title: title)).ToList();

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
        if (description.Length > 4_000)
            description = description[..4_000];
        return description;
    }

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(
            new DateOnly(1960, 1, 1),
            new DateOnly(2026, 12, 31)
        ).Year;

    public int GetValidDuration()
        => Faker.Random.Int(50, 300);

    public ERating GetRandomRating()
        => Faker.Random.Enum<ERating>();

    public string GetValidImagePath()
        => Faker.Image.PlaceImgUrl();

    public string GetValidMediaPath()
    {
        var exampleMedias = new[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/file.mp4",
            "https://www.s3.com.br/file-example.mp4",
            "https://www.globlstorage.com/file.mp4"
        };
        return exampleMedias[Random.Shared.Next(exampleMedias.Length)];
    }

    public Media GetValidMedia()
        => new(GetValidMediaPath());

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 10_000)
            description = description[..10_000];
        return description;
    }

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public List<Category> GetExampleCategoriesList(int count = 10)
        => Enumerable.Range(1, count).Select(_ => GetExampleCategory()).ToList();

    public string GetValidGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];
        if (genreName.Length > 255)
            genreName = genreName[..255];
        return genreName;
    }

    public Genre GetExampleGenre()
        => Genre.Create(
            GetValidGenreName(),
            GetRandomBoolean()
        );

    public List<Genre> GetExampleGenresList(int count = 10)
        => Enumerable.Range(1, count).Select(_ => GetExampleGenre()).ToList();

    public string GetValidCastMemberName()
    {
        var name = "";
        while (name.Length < 3)
            name = Faker.Name.FullName();
        if (name.Length > 255)
            name = name[..255];
        return name;
    }

    public ECastMemberType GetRandomCastMemberType()
        => Faker.Random.Enum<ECastMemberType>();

    public CastMember GetExampleCastMember()
        => new(
            GetValidCastMemberName(),
            GetRandomCastMemberType()
        );

    public List<CastMember> GetExampleCastMembersList(int count = 10)
        => Enumerable.Range(1, count).Select(_ => GetExampleCastMember()).ToList();
}

