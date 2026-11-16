
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;

public class UpdateMediaStatus : IUpdateMediaStatus
{
    private readonly IVideoRepository _videoRepo;
    private readonly IUnitOfWork _uow;
    private ILogger<UpdateMediaStatus> _logger;


    public UpdateMediaStatus(IVideoRepository videoRepoMock, IUnitOfWork uowMock, ILogger<UpdateMediaStatus> logger)
    {
        _videoRepo = videoRepoMock;
        _uow = uowMock;
        _logger = logger;
    }

    public async Task<VideoModelOutput> Handle(
        UpdateMediaStatusInput request,
        CancellationToken cancellationToken
    )
    {
        var video = await _videoRepo.Get(request.VideoId, cancellationToken);

        switch (request.Status)
        {
            case EMediaStatus.Completed:
                video.UpdateAsEncoded(request.EncodedPath!);
                break;
            case EMediaStatus.Error:
                _logger.LogError(
                    "There was an error while trying to encode the video {videoId}: {error}",
                    video.Id, request.ErrorMessage
                );
                video.UpdateAsEncodingError();
                break;
            default:
                throw new EntityValidationException("Invalid Media Status");
        }

        await _videoRepo.Update(video, cancellationToken);
        await _uow.Commit(cancellationToken);

        return VideoModelOutput.FromEntity(video);
    }
}
