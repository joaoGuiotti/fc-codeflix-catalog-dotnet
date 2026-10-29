using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTestFixtureCollection : ICollectionFixture<GetGenreApiTestFixture>
{ }

public class GetGenreApiTestFixture : GenreApiBaseFixture
{
    public Domain.Entity.Genre GetExampleGenre()
       => new(GetValidName());

    public List<Domain.Entity.Genre> GetExampleGenreList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ => Domain.Entity.Genre.Create(GetValidName()))
            .ToList();

    public List<Guid> GetExampleGuidList(int length = 22)
        => Enumerable.Range(0, 10)
            .Select(_ => Guid.NewGuid()).ToList();


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

    public List<Domain.Entity.Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ => Domain.Entity.Category.Create(GetValidName(), Faker.Commerce.ProductDescription()))
            .ToList();

    public void CompareOutputGenreCategories(IReadOnlyList<GenreModelOutputCategory> outputCategories, List<Domain.Entity.Category> categories)
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
