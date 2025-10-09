using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true)
        : base()
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        Validate();
    }

    public static Category Create(
        string name,
        string description,
        bool isActive = true
    )
    {
        return new Category(name, description, isActive);
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

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description ?? Description;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new EntityValidationException(
                $"{nameof(Name)} cannot be null or empty."
            );
        if (string.IsNullOrWhiteSpace(Description))
            throw new EntityValidationException(
                $"{nameof(Description)} cannot be null or empty."
            );
        if (Name.Length < 3)
            throw new EntityValidationException(
                $"{nameof(Name)} must be at least 3 characters long."
            );
        if (Name.Length > 255)
            throw new EntityValidationException(
                $"{nameof(Name)} must be less or equal 255 characters."
            );
        if (Description.Length > 10_000)
            throw new EntityValidationException(
                $"{nameof(Description)} must be less or equal 10.000 caracteres."
            );
    }
}
