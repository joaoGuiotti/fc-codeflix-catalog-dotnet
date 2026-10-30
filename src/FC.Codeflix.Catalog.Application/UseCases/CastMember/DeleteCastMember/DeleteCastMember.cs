using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

public class DeleteCastMember(
    ICastMemberRepository _repository,
    IUnitOfWork _uow
) : IDeleteCastMember
{
    public async Task Handle(DeleteCastMemberInput request, CancellationToken cancellationToken)
    {
        var aggregate = await _repository.Get(request.Id, cancellationToken);
        await _repository.Delete(aggregate, cancellationToken);
        await _uow.Commit(cancellationToken);
    }
}
