using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.SeedWork;

public abstract class Entity
{
    public Guid Id { get; private set; }

    protected Entity() => Id = Guid.NewGuid();

    private readonly Notification _notification = new();
    public INotification Notification => _notification;
}
