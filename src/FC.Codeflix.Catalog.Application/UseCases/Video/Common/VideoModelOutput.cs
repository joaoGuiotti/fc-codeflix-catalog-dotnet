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
            entity.Categories.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList().AsReadOnly(),
            entity.Genres.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList().AsReadOnly(),
            entity.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList().AsReadOnly(),
            entity.Thumb?.Path,
            entity.Banner?.Path,
            entity.ThumbHalf?.Path,
            entity.Media?.FilePath,
            entity.Trailer?.FilePath
        );

    public static VideoModelOutput FromEntity(
        Domain.Entity.Video entity,
        IReadOnlyList<Domain.Entity.Category>? categories = null,
        IReadOnlyList<Domain.Entity.Genre>? genres = null,
        IReadOnlyList<Domain.Entity.CastMember>? castMembers = null
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
            entity.Categories.Select(id => new VideoModelOutputRelatedAggregate(
                id,
                categories?.FirstOrDefault(x => x.Id == id)?.Name
            )).ToList(),
            entity.Genres.Select(id => new VideoModelOutputRelatedAggregate(
                id,
                genres?.FirstOrDefault(x => x.Id == id)?.Name
            )).ToList(),
            entity.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(
                id,
                castMembers?.FirstOrDefault(x => x.Id == id)?.Name
            )).ToList(),
            entity.Thumb?.Path,
            entity.Banner?.Path,
            entity.ThumbHalf?.Path,
            entity.Media?.FilePath,
            entity.Trailer?.FilePath
        );
}

public class VideoModelOutputRelatedAggregate
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public VideoModelOutputRelatedAggregate(Guid id, string? name = null)
        => (Id, Name) = (id, name);
}
