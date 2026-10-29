using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.EndToEndTests.Common;
using FluentAssertions;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

public class GenreApiBaseFixture : BaseFixture
{
    public GenreApiPersistence Persistence;

    public GenreApiBaseFixture()
    {
        Persistence = new GenreApiPersistence(CreateDbContext());
    }

    public string GetValidName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public bool GetRandomBoolean()
        => Faker.Random.Bool();

    public string GetInvalidNameTooShort()
        => Faker.Lorem.Letter(2);

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);

    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 1000)
            description = description[..1000];
        return description;
    }

    public Domain.Entity.Genre GetExampleGenre(
        bool? isActive = null,
        List<Guid>? categoriesIds = null,
        string? name = null
    )
    {
        var genre = new Domain.Entity.Genre(
            name ?? GetValidName(),
            isActive ?? GetRandomBoolean()
        );
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<Domain.Entity.Genre> GetExampleGenreList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ => GetExampleGenre())
            .ToList();

    public List<Guid> GetExampleGuidList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ => Guid.NewGuid())
            .ToList();

    public List<Domain.Entity.Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ => Domain.Entity.Category.Create(
                GetValidName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            ))
            .ToList();

    public void CompareEntityOutput(Domain.Entity.Genre genre, GenreModelOutput data)
    {
        genre.Should().NotBeNull();
        data.Should().NotBeNull();
        genre.Id.Should().Be(data.Id);
        genre.Name.Should().Be(data.Name);
        genre.IsActive.Should().Be(data.IsActive);
        genre.CreatedAt.Should().BeSameDateAs(data.CreatedAt);
        genre.Categories.Should().HaveCount(data.Categories.Count);
        foreach (var categoryId in genre.Categories)
            data.Categories.Should().Contain(x => x.Id == categoryId);
    }

    public void CompareOutputGenreCategories(
        IReadOnlyList<GenreModelOutputCategory> outputCategories,
        List<Domain.Entity.Category> categories
    )
    {
        outputCategories.Should().NotBeNull();
        outputCategories.Should().HaveCount(categories.Count);
        foreach (var categoryOutput in outputCategories)
        {
            var category = categories.Find(x => x.Id == categoryOutput.Id);
            category.Should().NotBeNull();
            category!.Name.Should().Be(categoryOutput.Name);
        }
    }
}
