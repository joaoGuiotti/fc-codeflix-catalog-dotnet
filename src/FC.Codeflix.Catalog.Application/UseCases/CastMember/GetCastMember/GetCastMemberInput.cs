using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;

public sealed record GetCastMemberInput(Guid Id) : IRequest<CastMemberOutput>
{
}
