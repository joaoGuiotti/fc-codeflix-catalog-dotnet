using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;

public class ListGenre : IListGenre
{
    private readonly IGenreRepository _repo;

    public ListGenre(IGenreRepository repo)
    {
        _repo = repo;
    }

    public async Task<ListGenreOutput> Handle(ListGenreInput request, CancellationToken cancellationToken)
    {
        var input = new SearchInput(
            page: request.Page,
            perPage: request.PerPage,
            search: request.Search,
            sort: request.Sort,
            sortDir: request.SortDir
        );
        var output = await _repo.Search(input, cancellationToken);
        return ListGenreOutput.FromSearchOutput(output);
    }
}
