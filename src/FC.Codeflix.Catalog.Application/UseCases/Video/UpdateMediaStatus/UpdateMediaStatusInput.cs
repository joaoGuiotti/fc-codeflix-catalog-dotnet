using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;

public record class UpdateMediaStatusInput(
    Guid VideoId,
    EMediaStatus Status,
    string? EncodedPath = null,
    string? ErrorMessage = null
) : IRequest<VideoModelOutput>;
