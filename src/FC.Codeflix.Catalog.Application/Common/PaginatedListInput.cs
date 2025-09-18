using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.Common;

public abstract class PaginatedListInput
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public string Search { get; set; }
    public string Sort { get; set; }
    public ESearchOrder SortDir { get; set; }

    protected PaginatedListInput(
        int page,
        int perPage,
        string search,
        string sort,
        ESearchOrder sortDir
    )
    {
        Page = page;
        PerPage = perPage;
        Search = search;
        Sort = sort;
        SortDir = sortDir;
    }
}
