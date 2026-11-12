using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FluentAssertions;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;

[CollectionDefinition(nameof(VideoApiBaseFixture))]
public class VideoApiBaseFixtureCollection : ICollectionFixture<VideoApiBaseFixture>
{
}

public class VideoApiBaseFixture : GenreApiBaseFixture
{
    public readonly VideoApiPersistence VideoPersistence;

    public readonly CastMemberApiPersistence CastMemberPersistence;

    public VideoApiBaseFixture() : base()
    {
        VideoPersistence = new VideoApiPersistence(DbContext);
        CastMemberPersistence = new CastMemberApiPersistence(DbContext);
    }

    public CreateVideoInput GetBasicCreateVideoInput()
        => new(
            Title: GetValidTitle(),
            Description: GetValidDescription(),
            Opened: GetRandomBoolean(),
            Published: GetRandomBoolean(),
            YearLaunched: GetValidYearLaunched(),
            Duration: GetValidDuration(),
            Rating: GetRandomRating()
        );

    public CreateVideoInput GetBasicCreateVideoInputWithRelations(
        List<Domain.Entity.Category> categories,
        List<Domain.Entity.Genre> genres,
        List<Domain.Entity.CastMember> castMembers
    )
       => new CreateVideoInput(
                Title: GetValidTitle(),
                Description: GetValidDescription(),
                Opened: GetRandomBoolean(),
                Published: GetRandomBoolean(),
                YearLaunched: GetValidYearLaunched(),
                Duration: GetValidDuration(),
                Rating: GetRandomRating(),
                CategoriesIds: categories.Select(x => x.Id).ToList(),
                GenresIds: genres.Select(x => x.Id).ToList(),
                CastMembersIds: castMembers.Select(x => x.Id).ToList()
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
        => Faker.Lorem.Letter(4_001);

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

    public Domain.Entity.Media GetValidMedia()
        => new(GetValidMediaPath());

    #region CastMembers
    public Domain.Entity.CastMember GetExampleCastMember(
        string? name = null,
        ECastMemberType? type = null
    ) => Domain.Entity.CastMember.Create(
        name ?? GetValidName(),
        type ?? GetRandomType()
    );

    public List<Domain.Entity.CastMember> GetExampleCastMembersList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ =>
            {
                Thread.Sleep(1);
                return GetExampleCastMember();
            })
        .ToList();

    public ECastMemberType GetRandomType()
       => Faker.Random.Enum<ECastMemberType>();
    #endregion

}
