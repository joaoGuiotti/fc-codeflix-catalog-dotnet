using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadVideo;

public record UpdateVideoInput(
    Guid Id,
    string Title,
    string Description,
    int YearLaunched,
    bool Opened,
    bool Published,
    int Duration,
    ERating Rating,
    List<Guid>? GenresIds = null,
    List<Guid>? CategoriesIds = null,
    List<Guid>? CastMembersIds = null,
    FileInput? Banner = null,
    FileInput? Thumb = null,
    FileInput? ThumbHalf = null
) : IRequest<VideoModelOutput>;
