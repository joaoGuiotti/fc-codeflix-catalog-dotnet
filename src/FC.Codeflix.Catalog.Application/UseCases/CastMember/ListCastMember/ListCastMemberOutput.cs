using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;

public class ListCastMemberOutput(
    int page,
    int perPage,
    int total,
    IReadOnlyList<CastMemberOutput> items
) : PaginatedListOutput<CastMemberOutput>(page, perPage, total, items)
{
    public static ListCastMemberOutput FromSearchOutput(
        SearchOutput<Domain.Entity.CastMember> searchOutput
    ) => new(
        searchOutput.CurrentPage,
        searchOutput.PerPage,
        searchOutput.Total,
        items: searchOutput
            .Items
            .Select(CastMemberOutput.FromEntity)
            .ToList()
    );
};
