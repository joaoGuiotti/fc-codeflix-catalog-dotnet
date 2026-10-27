using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
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


    public bool GetRandomBoolean()
        => Faker.Random.Bool();
}
