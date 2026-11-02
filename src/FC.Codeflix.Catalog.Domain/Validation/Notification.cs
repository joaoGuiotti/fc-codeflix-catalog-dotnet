namespace FC.Codeflix.Catalog.Domain.Validation;

public class Notification : INotification
{
    private readonly List<ValidationError> _errors = [];

    public bool HasErrors => _errors.Count > 0;
    public IReadOnlyCollection<ValidationError> Errors => _errors.AsReadOnly();

    public void Append(ValidationError error) => _errors.Add(error);
    public void Append(IEnumerable<ValidationError> errors) => _errors.AddRange(errors);
    public void Clear() => _errors.Clear();
}
