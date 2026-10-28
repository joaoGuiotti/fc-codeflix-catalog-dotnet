using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;

public class ListGenreOutput : PaginatedListOutput<GenreModelOutput>
{
    public ListGenreOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<GenreModelOutput> items
    )
        : base(page, perPage, total, items) { }

    public static ListGenreOutput FromSearchOutput(
        SearchOutput<Domain.Entity.Genre> searchOutput
    ) =>
        new(
            page: searchOutput.CurrentPage,
            perPage: searchOutput.PerPage,
            total: searchOutput.Total,
            items: searchOutput
                .Items.Select(GenreModelOutput.FromEntity)
                .ToList()
        );
}
