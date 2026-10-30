using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

public sealed record DeleteCastMemberInput(Guid Id)
    : IRequest;
