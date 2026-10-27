using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.IntegrationTests.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Moq;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

public class GenreUseCaseBaseFixture : BaseFixture
{
    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public Mock<IGenreRepository> GetGenreRepositoryMock()
        => new();

    public Mock<ICategoryRepository> GetCategoryrepositoryMock()
        => new();

    public UseCase.CreateGenre CreateGenreUseCase(CodeflixCatalogDbContext? dbContext = null)
    {
        if (dbContext is null)
            dbContext = CreateDbContext();
        var genreRepo = new GenreRepository(dbContext);
        var categoryRepo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);
        return new(genreRepo, uow, categoryRepo);
    }
}
