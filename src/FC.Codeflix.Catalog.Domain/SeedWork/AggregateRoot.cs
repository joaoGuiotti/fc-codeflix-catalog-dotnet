using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FC.Codeflix.Catalog.Domain.SeedWork;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot()
        : base() { }
}
