using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

public interface ICreateCastMember
    : IRequestHandler<CreateCastMemberInput, CastMemberOutput>
{
}

public record CastMemberOutput(Guid Id, string Name, ECastMemberType type, DateTime CreatedAt)
{
    public static CastMemberOutput FromEntity(Domain.Entity.CastMember entity)
        => new(entity.Id, entity.Name, entity.Type, entity.CreatedAt);
}
