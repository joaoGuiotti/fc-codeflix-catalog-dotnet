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
        SearchOutput<Domain.Entity.Video> searchOutput
    )
    {
        return new(
            page: searchOutput.CurrentPage,
            perPage: searchOutput.PerPage,
            total: searchOutput.Total,
            items: searchOutput
                .Items.Select(VideoModelOutput.FromEntity)
                .ToList()
        );
    }

    internal void FillWithReleatedAggregates(
        IReadOnlyList<Domain.Entity.Category>? categories = null,
        IReadOnlyList<Domain.Entity.Genre>? genres = null,
        IReadOnlyList<Domain.Entity.CastMember>? castMembers = null
    )
    {
        foreach (var item in Items)
        {

            foreach (var categoryOut in item.Categories)
                categoryOut.Name = categories?.FirstOrDefault(
                    category => category.Id == categoryOut.Id
                )?.Name;

            foreach (var genreOut in item.Genres)
                genreOut.Name = genres?.FirstOrDefault(
                    genre => genre.Id == genreOut.Id
                )?.Name;

            foreach (var castMemberOut in item.CastMembers)
                castMemberOut.Name = castMembers?.FirstOrDefault(
                    castMember => castMember.Id == castMemberOut.Id
                )?.Name;

        }
    }


}
