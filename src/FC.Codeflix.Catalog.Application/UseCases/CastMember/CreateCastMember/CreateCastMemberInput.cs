using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

public sealed record CreateCastMemberInput(
        string Name,
        ECastMemberType Type
    ) : IRequest<CastMemberOutput>
{ }
