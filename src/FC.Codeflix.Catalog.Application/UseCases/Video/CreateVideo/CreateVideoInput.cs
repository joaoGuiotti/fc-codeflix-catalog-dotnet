using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public record CreateVideoInput(
    string Title,
    string Description,
    bool Opened,
    bool Published,
    int YearLaunched,
    int Duration,
    ERating Rating,
    List<Guid>? CategoriesIds = null,
    List<Guid>? GenresIds = null,
    List<Guid>? CastMembersIds = null,
    FileInput? Thumb = null,
    FileInput? Banner = null,
    FileInput? ThumbHalf = null
) : IRequest<CreateVideoOutput>;
