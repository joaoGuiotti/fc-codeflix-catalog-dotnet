using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.ListCastMember;

[CollectionDefinition(nameof(ListCastMemberTestFixture))]
public class ListCastMemberTestFixtureCollection
    : ICollectionFixture<ListCastMemberTestFixture>
{ }

public class ListCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
    public UseCase.ListCastMemberInput GetValidInput()
        => new(
            Faker.Random.Int(1, 10),
            Faker.Random.Int(15, 100),
            Faker.Lorem.Word(),
            Faker.PickRandom(new List<string> { "name", "type", "createdAt" }),
            Faker.PickRandom(
                new List<ESearchOrder> { ESearchOrder.Asc, ESearchOrder.Desc }
            )
        );
}
