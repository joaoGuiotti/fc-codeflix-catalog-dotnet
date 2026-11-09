using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;

public class ListVideoOutput : PaginatedListOutput<VideoModelOutput>
{
    public ListVideoOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<VideoModelOutput> items
    )
        : base(page, perPage, total, items) { }

    public static ListVideoOutput FromSearchOutput(
        SearchOutput<Domain.Entity.Video> searchOutput,
        IReadOnlyList<Domain.Entity.Category>? categories = null,
        IReadOnlyList<Domain.Entity.Genre>? genres = null,
        IReadOnlyList<Domain.Entity.CastMember>? castMembers = null
    )
    {
        return new(
            page: searchOutput.CurrentPage,
            perPage: searchOutput.PerPage,
            total: searchOutput.Total,
            items: searchOutput
                .Items.Select(video => VideoModelOutput.FromEntity(video, categories, genres, castMembers))
                .ToList()
        );
    }
}
