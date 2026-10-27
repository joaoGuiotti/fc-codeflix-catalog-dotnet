using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Genre : AggregateRoot
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<Guid> Categories
        => _categories.AsReadOnly();
    private List<Guid> _categories;

    public Genre(
        string name,
        bool isActive = true
    )
        : base()
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        _categories = new List<Guid>();
        Validate();
    }

    public static Genre Create(
        string name,
        bool isActive = true
    ) => new(name, isActive);

    public void Update(string name)
    {
        Name = name;
        Validate();
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new EntityValidationException(
                $"{nameof(Name)} cannot be null or empty."
            );
        if (Name.Length < 3)
            throw new EntityValidationException(
                $"{nameof(Name)} must be at least 3 characters long."
            );
        if (Name.Length > 255)
            throw new EntityValidationException(
                $"{nameof(Name)} must be less or equal 255 characters."
            );
    }

    public void AddCategory(Guid categoryGuid)
    {
        _categories.Add(categoryGuid);
        Validate();
    }

    public void RemoveCategory(Guid categoryGuid)
    {
        _categories.Remove(categoryGuid);
        Validate();
    }

    public void RemoveAllCategories()
    {
        _categories = new List<Guid>();
        Validate();
    }
}
