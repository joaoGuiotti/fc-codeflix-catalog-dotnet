using FC.Codeflix.Catalog.EndToEndTests.Common;

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
}
