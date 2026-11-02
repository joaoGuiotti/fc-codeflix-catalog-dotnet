namespace FC.Codeflix.Catalog.Domain.Validation;

public interface INotification
{
    IReadOnlyCollection<ValidationError> Errors { get; }
    void Append(ValidationError error);
    void Append(IEnumerable<ValidationError> errors);
    public bool HasErrors => Errors.Count > 0;
}
