using FluentValidation.Results;

namespace FC.Codeflix.Catalog.Domain.Exceptions;

public class EntityValidationException : Exception
{
    public IReadOnlyCollection<ValidationFailure>? Errors { get; }

    public EntityValidationException(
        string? message,
        IReadOnlyCollection<ValidationFailure>? errors = null
    ) : base(message)
    {
        Errors = errors;
    }
}
