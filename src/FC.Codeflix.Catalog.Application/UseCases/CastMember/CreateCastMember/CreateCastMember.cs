using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

public class CreateCastMember : ICreateCastMember
{
    private readonly ICastMemberRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateCastMember(ICastMemberRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<CastMemberOutput> Handle(
        CreateCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var castMember = Domain.Entity.CastMember.Create(request.Name, request.Type);

        await _repo.Insert(castMember, cancellationToken);
        await _uow.Commit(cancellationToken);

        return CastMemberOutput.FromEntity(castMember);
    }
}
