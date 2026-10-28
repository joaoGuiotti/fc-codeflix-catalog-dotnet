using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[CollectionDefinition(nameof(GenreTestFixtureCollection))]
public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture>
{ }

public class GenreTestFixture : BaseFixture
{
    public string GetValidName()
            => Faker.Name.FirstName();

    public bool GetRandomBool()
           => Faker.Random.Bool();

    public DomainEntity.Genre GetValidGenre()
        => DomainEntity.Genre.Create(GetValidName(), GetRandomBool());

    public DomainEntity.Genre GetValidGenreWithCategories(int categoriesLenght = 12)
    {
        var genre = GetValidGenre();
        for (int i = 0; i < categoriesLenght; i++)
            genre.AddCategory(Guid.NewGuid());
        return genre;
    }

}
