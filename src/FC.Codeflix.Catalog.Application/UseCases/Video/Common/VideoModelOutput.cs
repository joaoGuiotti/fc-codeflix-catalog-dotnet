using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;

public record VideoModelOutput(
    Guid Id,
    string Title,
    string Description,
    bool Opened,
    bool Published,
    int YearLaunched,
    int Duration,
    ERating Rating,
    DateTime CreatedAt,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> Categories,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> Genres,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> CastMembers,
    string? Thumb,
    string? Banner,
    string? ThumbHalf,
    string? Media,
    string? Trailer
)
{
    public static VideoModelOutput FromEntity(Domain.Entity.Video entity)
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
            entity.Categories.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            entity.Genres.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            entity.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            entity.Thumb?.Path,
            entity.Banner?.Path,
            entity.ThumbHalf?.Path,
            entity.Media?.FilePath,
            entity.Trailer?.FilePath
        );

    public static VideoModelOutput FromEntity(
        Domain.Entity.Video entity,
        IReadOnlyList<Domain.Entity.Category>? categories = null,
        IReadOnlyList<Domain.Entity.Genre>? genres = null
    )
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
            categories?.Select(x => new VideoModelOutputRelatedAggregate(x.Id, x.Name)).ToList()
                ?? entity.Categories.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            genres?.Select(x => new VideoModelOutputRelatedAggregate(x.Id, x.Name)).ToList()
                ?? entity.Genres.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            entity.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            entity.Thumb?.Path,
            entity.Banner?.Path,
            entity.ThumbHalf?.Path,
            entity.Media?.FilePath,
            entity.Trailer?.FilePath
        );
}

public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);
