using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;

public class ListCastMemberInput : PaginatedListInput
    , IRequest<ListCastMemberOutput>
{
    public ListCastMemberInput() :
        base(1, 15, "", "", ESearchOrder.Asc)
    {
    }

    public ListCastMemberInput(
        int page = 1,
        int perPage = 15,
        string? search = "",
        string? sort = "",
        ESearchOrder dir = ESearchOrder.Asc
    )
        : base(page, perPage, search, sort, dir)
    {
    }
};
