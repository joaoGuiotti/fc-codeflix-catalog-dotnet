
namespace FC.Codeflix.Catalog.Application.UseCases.Genre.Common;

public class GenreModelOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyList<Guid> Categories { get; set; }
    public DateTime CreatedAt { get; set; }

    public GenreModelOutput(
        Guid id,
        string name,
        bool isActive,
        IReadOnlyList<Guid> categories,
        DateTime createdAt
    )
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        Categories = categories;
        CreatedAt = createdAt;
    }

    public static GenreModelOutput FromEntity(
        Domain.Entity.Genre entity
    ) =>
        new(
            entity.Id,
            entity.Name,
            entity.IsActive,
            entity.Categories,
            entity.CreatedAt
        );

}
