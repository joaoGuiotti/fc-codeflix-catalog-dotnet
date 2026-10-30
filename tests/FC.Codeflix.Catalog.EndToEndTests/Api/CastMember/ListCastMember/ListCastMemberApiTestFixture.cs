using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMember;

[CollectionDefinition(nameof(ListCastMemberApiTestFixture))]
public class ListCastMemberApiTestFixtureCollection
    : ICollectionFixture<ListCastMemberApiTestFixture>
{ }

public class ListCastMemberApiTestFixture : CastMemberApiBaseFixture
{
    public List<DomainEntity.CastMember> CloneCastMembersListOrdered(
        List<DomainEntity.CastMember> castMembersList,
        string orderBy,
        ESearchOrder order
    )
    {
        var listClone = new List<DomainEntity.CastMember>(castMembersList);
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", ESearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Name).ThenBy(x => x.Id),
            ("type", ESearchOrder.Asc) => listClone.OrderBy(x => x.Type).ThenBy(x => x.Id),
            ("type", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Type).ThenBy(x => x.Id),
            ("id", ESearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", ESearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
            ("createdat", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id),
            (_, ESearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
            _ => listClone.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}
