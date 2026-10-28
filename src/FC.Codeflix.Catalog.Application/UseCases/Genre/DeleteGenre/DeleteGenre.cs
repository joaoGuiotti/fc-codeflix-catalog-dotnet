using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

public class DeleteGenre : IDeleteGenre
{
    private readonly IGenreRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteGenre(IGenreRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task Handle(DeleteGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _repo.Get(request.Id, cancellationToken);
        await _repo.Delete(genre, cancellationToken);
        await _uow.Commit(cancellationToken);
    }
}
