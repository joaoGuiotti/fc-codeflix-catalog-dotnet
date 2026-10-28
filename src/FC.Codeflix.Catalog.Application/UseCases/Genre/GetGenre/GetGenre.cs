using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;

public class GetGenre : IGetGenre
{
    private readonly IGenreRepository _repo;

    public GetGenre(IGenreRepository repo)
        => _repo = repo;

    public async Task<GenreModelOutput> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _repo.Get(request.Id, cancellationToken);
        return GenreModelOutput.FromEntity(genre);
    }
}
