using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

public class ListCategories : IListCategories
{
    private readonly ICategoryRepository _repo;

    public ListCategories(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<ListCategoriesOutput> Handle(
        ListCategoriesInput request,
        CancellationToken cancellationToken
    )
    {
        var input = new SearchInput(
            page: request.Page,
            perPage: request.PerPage,
            search: request.Search,
            sort: request.Sort,
            sortDir: request.SortDir
        );

        var searchOutput = await _repo.Search(input, cancellationToken);

        return ListCategoriesOutput.FromSearchOutput(searchOutput);
    }
}
