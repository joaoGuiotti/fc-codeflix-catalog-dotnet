using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Api.Contracts.CastMembers;

public sealed record UpdateCastMemberRequest(
    string Name,
    ECastMemberType? Type
);

public static class UpdateCastMemberMappings
{
    public static UpdateCastMemberInput ToInput(this UpdateCastMemberRequest request, Guid id)
        => new(id, request.Name, request.Type);
}
