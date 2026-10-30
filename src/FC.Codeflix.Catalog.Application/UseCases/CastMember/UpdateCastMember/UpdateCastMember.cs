using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Repository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public class UpdateCastMember(ICastMemberRepository repository, IUnitOfWork uow) : IUpdateCastMember
{
    public async Task<CastMemberOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await repository.Get(request.Id, cancellationToken);

        castMember.Update(request.Name, request.Type);

        await repository.Update(castMember, cancellationToken);
        await uow.Commit(cancellationToken);

        return CastMemberOutput.FromEntity(castMember);
    }
}
