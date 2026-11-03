using Bogus;
using FC.Codeflix.Catalog.Application.Interfaces;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Common;

public abstract class BaseFixture
{
    protected BaseFixture() => Faker = new Faker("pt_BR");

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public Faker Faker { get; set; }
}
