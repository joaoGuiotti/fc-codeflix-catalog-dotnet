using FC.Codeflix.Catalog.Application.UseCases.Video.UploadVideo;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Api.Models.Video;

public class UpdateVideoApiInput
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int YearLaunched { get; set; }
    public bool Opened { get; set; }
    public bool Published { get; set; }
    public int Duration { get; set; }
    public ERating? Rating { get; set; }
    public List<Guid>? CategoriesId { get; set; }
    public List<Guid>? GenresId { get; set; }
    public List<Guid>? CastMembersId { get; set; }

    public UpdateVideoInput ToInput(Guid id)
        => new UpdateVideoInput(
            id,
            Title,
            Description,
            YearLaunched,
            Opened,
            Published,
            Duration,
            Rating.Value,
            GenresId,
            CategoriesId,
            CastMembersId
        );
}
