using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;

public class GetCastMember : IGetCastMember
{
    private readonly ICastMemberRepository _repo;

    public GetCastMember(ICastMemberRepository repo)
    {
        _repo = repo;
    }

    public async Task<CastMemberOutput> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _repo.Get(request.Id, cancellationToken);
        return CastMemberOutput.FromEntity(castMember);
    }
}
