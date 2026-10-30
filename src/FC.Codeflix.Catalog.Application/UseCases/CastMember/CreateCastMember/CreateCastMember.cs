using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

public class CreateCastMember(
    ICastMemberRepository repo,
    IUnitOfWork uow
) : ICreateCastMember
{
    public async Task<CastMemberOutput> Handle(
        CreateCastMemberInput request,
        CancellationToken cancellationToken
    )
    {
        var castMember = Domain.Entity.CastMember.Create(request.Name, request.Type);

        await repo.Insert(castMember, cancellationToken);
        await uow.Commit(cancellationToken);

        return CastMemberOutput.FromEntity(castMember);
    }
}
