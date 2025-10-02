using Bogus;

namespace FC.Codeflix.Catalog.IntegrationTests.Common;

public class BaseFixture
{

    protected Faker Faker { get; set; }

    public BaseFixture()
        => Faker = new Faker("pt_BR");

}
