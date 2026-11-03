namespace FC.Codeflix.Catalog.Domain.Validation;

public class Notification : INotification
{
    private readonly Dictionary<(string? Property, string Message), ValidationError> _errors = new();

    public IReadOnlyList<ValidationError> Errors => _errors.Values.ToList().AsReadOnly();

    public void AddError(string errorMessage, string? propertyName = null)
    {
        _errors.TryAdd((propertyName, errorMessage), new ValidationError(errorMessage, propertyName));
    }

    public bool HasErrors() => _errors.Count > 0;

    public void Clear() => _errors.Clear();
}
