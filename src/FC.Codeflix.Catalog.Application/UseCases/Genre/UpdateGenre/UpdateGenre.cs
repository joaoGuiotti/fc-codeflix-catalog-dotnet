using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

public class UpdateGenre : IUpdateGenre
{
    private readonly IGenreRepository _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IUnitOfWork _uow;

    public UpdateGenre(
        IGenreRepository repo,
        IUnitOfWork uow,
        ICategoryRepository repoCategory
    )
    {
        _repo = repo;
        _uow = uow;
        _categoryRepo = repoCategory;
    }

    public async Task<GenreModelOutput> Handle(
        UpdateGenreInput request,
        CancellationToken cancellationToken
    )
    {
        var genre = await _repo.Get(request.Id, cancellationToken);

        genre.Update(request.Name);

        if (request.isActive == true)
            genre.Activate();
        else if (request.isActive == false)
            genre.Deactivate();

        if (request.CategoriesIds is not null && request.CategoriesIds.Count > 0)
        {
            await ValidateCategoriesIds(request, cancellationToken);
            genre.RemoveAllCategories();
            request.CategoriesIds.ForEach(genre.AddCategory);
        }

        await _repo.Update(genre, cancellationToken);
        await _uow.Commit(cancellationToken);

        return GenreModelOutput.FromEntity(genre);
    }

    private async Task ValidateCategoriesIds(
       UpdateGenreInput request,
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
