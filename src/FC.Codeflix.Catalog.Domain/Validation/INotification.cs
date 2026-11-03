namespace FC.Codeflix.Catalog.Domain.Validation;

public interface INotification
{
    IReadOnlyList<ValidationError> Errors { get; }
    void AddError(string errorMessage, string? propertyName = null);
    public bool HasErrors();
    public void Clear();
}
