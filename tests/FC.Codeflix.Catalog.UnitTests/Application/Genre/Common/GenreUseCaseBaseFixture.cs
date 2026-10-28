using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.Common;

public class GenreUseCaseBaseFixture : BaseFixture
{
    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public Mock<IGenreRepository> GetGenreRepositoryMock()
        => new();

    public Mock<ICategoryRepository> GetCategoryrepositoryMock()
        => new();

    public string GetValidName()
       => Faker.Commerce.Categories(1)[0];

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);

    public DomainEntity.Genre GetExampleGenre(
        bool? isActive = null,
        List<Guid>? categoriesIds = null
    )
    {
        var genre = new DomainEntity.Genre(
            GetValidName(),
            isActive ?? GetRandomBoolean()
        );

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable
            .Range(1, count ?? (new Random()).Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();


    public bool GetRandomBoolean()
        => Faker.Random.Bool();
}
