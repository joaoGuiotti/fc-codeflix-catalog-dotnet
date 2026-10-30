using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;

public interface IListCastMember
    : IRequestHandler<ListCastMemberInput, ListCastMemberOutput>;
