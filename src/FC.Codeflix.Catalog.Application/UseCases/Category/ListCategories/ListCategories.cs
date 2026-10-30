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
        var searchOutput = await _repo.Search(request.ToSearchInput(), cancellationToken);
        return ListCategoriesOutput.FromSearchOutput(searchOutput);
    }
}
