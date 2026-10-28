using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _uow;

    public CreateGenre(IGenreRepository repo, IUnitOfWork uow, ICategoryRepository categoryRepo)
    {
        _repo = repo;
        _uow = uow;
        _categoryRepo = categoryRepo;
    }

    public async Task<GenreModelOutput> Handle(
        CreateGenreInput request,
        CancellationToken cancellationToken
    )
    {
        var genre = DomainEntity.Genre.Create(
            request.Name,
            request.IsActive
        );

        if (request.CategoriesIds is not null && request.CategoriesIds.Count > 0)
        {
            await ValidateCategoriesIds(request, cancellationToken);
            request.CategoriesIds
                .ForEach(genre.AddCategory);
        }

        await _repo.Insert(genre, cancellationToken);
        await _uow.Commit(cancellationToken);

        return GenreModelOutput.FromEntity(genre);
    }

    private async Task ValidateCategoriesIds(
        CreateGenreInput request,
        CancellationToken cancellationToken
    )
    {
        var idsInPersistence = await _categoryRepo.GetIdsListById(
            request.CategoriesIds!,
            cancellationToken
        );

        RelatedAggregateException.ThrowIfNotFound(
            request.CategoriesIds!,
            idsInPersistence,
            nameof(DomainEntity.Category)
        );
    }
}
