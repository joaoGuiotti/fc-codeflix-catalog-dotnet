using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class CastMember : AggregateRoot
{
    private CastMember(string name, ECastMemberType type)
        : base()
    {
        Name = name;
        CreatedAt = DateTime.Now;
        Type = type;

        Validate();
    }

    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public ECastMemberType Type { get; set; }

    public static CastMember Create(string name, ECastMemberType type)
        => new(name, type);

    public static CastMember CreateAnActor(string name)
        => new(name, ECastMemberType.Actor);

    public static CastMember CreateAnDirector(string name)
        => new(name, ECastMemberType.Director);

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
}
