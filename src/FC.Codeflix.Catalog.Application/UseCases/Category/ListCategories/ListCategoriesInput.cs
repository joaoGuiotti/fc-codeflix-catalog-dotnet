using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

public class ListCategoriesInput
    : PaginatedListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(
        int page = 1,
        int perPage = 15,
        string search = "",
        string sort = "",
        ESearchOrder sortDir = ESearchOrder.Asc
    ) : base(page, perPage, search, sort, sortDir)
    { }
}
