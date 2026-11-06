using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideo;

[CollectionDefinition(nameof(ListVideoTestFixture))]
public class ListVideoTestFixtureCollection : ICollectionFixture<ListVideoTestFixture>;

public class ListVideoTestFixture : VideoTestFixtureBase
{
    public List<DomainEntity.Video> GetExampleVideosList(int? length = null)
        => Enumerable
            .Range(1, length ?? Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

    public List<DomainEntity.Video> GetExampleVideosListWithoutRelations(int? length = null)
        => Enumerable
            .Range(1, length ?? Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideo())
            .ToList();

    public (
        List<DomainEntity.Video> Videos,
        List<DomainEntity.Category> Categories,
        List<DomainEntity.Genre> Genres,
        List<DomainEntity.CastMember> CastMembers
    ) GetExampleVideosListWithRelations()
    {
        var itemsQuantityToBeCreated = Random.Shared.Next(2, 10);
        var categories = new List<DomainEntity.Category>();
        var genres = new List<DomainEntity.Genre>();
        var castMembers = new List<DomainEntity.CastMember>();
        var videos = Enumerable
            .Range(1, itemsQuantityToBeCreated)
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

        videos.ForEach(video =>
        {
            video.RemoveAllCategory();
            var qtdCategories = Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdCategories; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres = Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdGenres; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }

            video.RemoveAllCastMembers();
            var qtdCastMembers = Random.Shared.Next(2, 5);
            for (var i = 0; i < qtdCastMembers; i++)
            {
                var castMember = GetExampleCastMember();
                castMembers.Add(castMember);
                video.AddCastMember(castMember.Id);
            }
        });

        return (videos, categories, genres, castMembers);
    }

    private string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    private string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public DomainEntity.Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    private string GetValidGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];
        if (genreName.Length > 255)
            genreName = genreName[..255];
        return genreName;
    }

    public DomainEntity.Genre GetExampleGenre()
        => new(
            GetValidGenreName(),
            GetRandomBoolean()
        );

    public DomainEntity.CastMember GetExampleCastMember()
        => new(
             name: Faker.Person.FullName,
             type: Faker.Random.Enum<ECastMemberType>()
        );

    public ListVideoInput GetValidInput()
        => new(
            page: Faker.Random.Int(1, 10),
            perPage: Faker.Random.Int(15, 100),
            search: Faker.Lorem.Word(),
            sort: Faker.PickRandom(new List<string> { "title", "createdAt" }),
            sortDir: Faker.PickRandom(
                new List<ESearchOrder> { ESearchOrder.Asc, ESearchOrder.Desc }
            )
        );
}

