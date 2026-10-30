using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public interface IUpdateCastMember
    : IRequestHandler<UpdateCastMemberInput, CastMemberOutput>
{
}



