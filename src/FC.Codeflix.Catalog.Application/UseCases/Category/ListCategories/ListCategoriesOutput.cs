using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

public class ListCategoriesOutput
    : PaginatedListOutput<CategoryModelOutput>
{
    public ListCategoriesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<CategoryModelOutput> items
    ) : base(page, perPage, total, items)
    { }

    public static ListCategoriesOutput FromSearchOutput(SearchOutput<Domain.Entity.Category> searchOutput)
        => new ListCategoriesOutput(
            page: searchOutput.CurrentPage,
            perPage: searchOutput.PerPage,
            total: searchOutput.Total,
            items: searchOutput.Items
                .Select(CategoryModelOutput.FromCategory)
                .ToList()
        );
}
