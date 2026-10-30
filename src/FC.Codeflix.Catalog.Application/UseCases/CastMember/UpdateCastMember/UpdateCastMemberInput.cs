using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public sealed record UpdateCastMemberInput(
    Guid Id,
    string Name,
    ECastMemberType? Type
) : IRequest<CastMemberOutput>;
