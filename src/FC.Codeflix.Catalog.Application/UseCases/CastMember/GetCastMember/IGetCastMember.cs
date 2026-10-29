using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;

public interface IGetCastMember
    : IRequestHandler<GetCastMemberInput, CastMemberOutput>
{
}
