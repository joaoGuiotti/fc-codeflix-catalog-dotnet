using System.Diagnostics.CodeAnalysis;

namespace FC.Codeflix.Catalog.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message)
        : base(message) { }

    public static void ThrowIfNull<T>(
        [NotNull] T? @object,
        Guid Id
    ) where T : class
    {
        if (@object is null)
            throw new NotFoundException($"{typeof(T).Name} with ID {Id} not found.");
    }
}
