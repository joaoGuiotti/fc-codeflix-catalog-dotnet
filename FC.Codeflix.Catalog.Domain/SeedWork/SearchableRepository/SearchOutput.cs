namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

public class SearchOutput<TAggregate>
    where TAggregate : AggregateRoot
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public int LastPage => (int)Math.Ceiling((decimal)Total / PerPage);
    public IReadOnlyList<TAggregate> Items { get; set; }

    public SearchOutput(
        int perPage,
        int currentPage,
        int total,
        IReadOnlyList<TAggregate> items
    )
    {
        PerPage = perPage;
        CurrentPage = currentPage;
        Total = total;
        Items = items;
    }
}
