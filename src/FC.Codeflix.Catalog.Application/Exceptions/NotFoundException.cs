using FC.Codeflix.Catalog.Domain.SeedWork;
using System.Diagnostics.CodeAnalysis;

namespace FC.Codeflix.Catalog.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message)
        : base(message) { }


    public static void ThrowIfNull<TEntity>(
        [NotNull] TEntity? entity,
        Guid Id
    ) where TEntity : AggregateRoot
    {
        if (entity is null)
            throw new NotFoundException($"{typeof(TEntity).Name} with ID {Id} not found.");
    }
}
