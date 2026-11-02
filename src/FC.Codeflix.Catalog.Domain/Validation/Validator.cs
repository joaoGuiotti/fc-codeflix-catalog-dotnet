namespace FC.Codeflix.Catalog.Domain.Validation;

public abstract class Validator(INotification notification)
{
    protected INotification Notification { get; } = notification;
    public abstract void Validate();
}
