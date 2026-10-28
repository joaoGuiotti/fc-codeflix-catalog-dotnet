
namespace FC.Codeflix.Catalog.Application.UseCases.Genre.Common;

public class GenreModelOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyList<GenreModelOutputCategory> Categories { get; set; }
    public DateTime CreatedAt { get; set; }

    public GenreModelOutput(
        Guid id,
        string name,
        bool isActive,
        IReadOnlyList<GenreModelOutputCategory> categories,
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
            entity.Categories.Select(
                    categoryId => new GenreModelOutputCategory(categoryId)
            ).ToList().AsReadOnly(),
            entity.CreatedAt
        );
}

public class GenreModelOutputCategory
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public GenreModelOutputCategory(Guid id, string? name = null)
        => (Id, Name) = (id, name);
}
