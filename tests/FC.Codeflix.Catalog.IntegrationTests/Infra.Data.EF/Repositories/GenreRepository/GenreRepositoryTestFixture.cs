using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTests.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection
    : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture : BaseFixture
{
    public string GetValidGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];
        if (genreName.Length > 255)
            genreName = genreName[..255];
        return genreName;
    }

    public Genre GetExampleGenre(
        bool? isActive = null,
        List<Guid>? categoriesIds = null,
        string? name = null
    )
    {
        var genre = Genre.Create(
            name ?? GetValidGenreName(),
            isActive ?? GetRandomBoolean()
        );
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<Genre> GetExampleGenresList(int length = 10) =>
        Enumerable.Range(1, length).Select(_ => GetExampleGenre()).ToList();

    public List<Genre> GetExampleGenresListByNames(List<string> names) =>
        names.Select(name => GetExampleGenre(name: name)).ToList();

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

    public Category GetExampleCategory(
        string? name = null,
        string? description = null,
        bool? isActive = null
    ) =>
        new(
            name ?? GetValidCategoryName(),
            description ?? GetValidCategoryDescription(),
            isActive ?? GetRandomBoolean()
        );

    public List<Category> GetExampleCategoriesList(int length = 10) =>
        Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
}

