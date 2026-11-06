using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;

public class ListVideoInput
    : PaginatedListInput, IRequest<ListVideoOutput>
{
    public ListVideoInput()
        : base(1, 15, String.Empty, String.Empty, ESearchOrder.Asc)
    {
    }

    public ListVideoInput(
        int page = 1,
        int perPage = 15,
        string? search = default,
        string? sort = default,
        ESearchOrder sortDir = ESearchOrder.Asc
    ) : base(page, perPage, search, sort, sortDir)
    {
    }
}
