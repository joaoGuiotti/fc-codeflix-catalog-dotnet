using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;

public class ListCastMember(
    ICastMemberRepository _repo
) : IListCastMember
{
    public async Task<ListCastMemberOutput> Handle(
        ListCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var input = new SearchInput(
            request.Page,
            request.PerPage,
            request.Search,
            request.Sort,
            request.SortDir
        );
        var searchOutput = await _repo.Search(input, cancellationToken);

        return ListCastMemberOutput.FromSearchOutput(searchOutput);
    }
}
