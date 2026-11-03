using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public record CreateVideoOutput(
    Guid Id,
    string Title,
    string Description,
    bool Opened,
    bool Published,
    int YearLaunched,
    int Duration,
    ERating Rating,
    DateTime CreatedAt
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
            entity.CreatedAt
        );
}
