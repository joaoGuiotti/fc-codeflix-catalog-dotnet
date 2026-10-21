namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

public class SearchInput
{
    public int Page { get; }
    public int PerPage { get; }
    public string? Search { get; }
    public string? Sort { get; }
    public ESearchOrder? SortDir { get; }

    public SearchInput(
        int page,
        int perPage,
        string? search = null,
        string? sort = null,
        ESearchOrder? sortDir = null
    )
    {
        Page = page;
        PerPage = perPage;
        Search = search;
        Sort = sort;
        SortDir = sortDir;
    }
}
