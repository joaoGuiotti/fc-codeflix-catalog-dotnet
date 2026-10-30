using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;

public class ListGenreInput
 : PaginatedListInput,
        IRequest<ListGenreOutput>
{
    public ListGenreInput()
        : base(1, 15, "", "", ESearchOrder.Asc) { }

    public ListGenreInput(
        int page = 1,
        int perPage = 15,
        string? search = "",
        string? sort = "",
        ESearchOrder sortDir = ESearchOrder.Asc
    )
        : base(page, perPage, search, sort, sortDir) { }
}
