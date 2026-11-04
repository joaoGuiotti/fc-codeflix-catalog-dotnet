using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;

public record CreateVideoOutput(
    Guid Id,
    string Title,
    string Description,
    bool Opened,
    bool Published,
    int YearLaunched,
    int Duration,
    ERating Rating,
    DateTime CreatedAt,
    IReadOnlyCollection<Guid> CategoriesIds,
    IReadOnlyCollection<Guid> GenresIds,
    IReadOnlyCollection<Guid> CastMembersIds,
    string? Thumb,
    string? Banner,
    string? ThumbHalf
)
{
    public static CreateVideoOutput FromEntity(Domain.Entity.Video entity)
        => new(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Opened,
            entity.Published,
            entity.YearLaunched,
            entity.Duration,
            entity.Rating,
            entity.CreatedAt,
            entity.Categories,
            entity.Genres,
            entity.CastMembers,
            entity.Thumb?.Path,
            entity.Banner?.Path,
            entity.ThumbHalf?.Path
        );

}
