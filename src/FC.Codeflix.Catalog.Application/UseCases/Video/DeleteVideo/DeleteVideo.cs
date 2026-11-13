using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;

public class DeleteVideo : IDeleteVideo
{
    private readonly IVideoRepository _repository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVideo(IVideoRepository repository, IStorageService storageService, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _repository.Get(request.Id, cancellationToken);

        var mediaPath = video.Media?.FilePath;
        var trailerPath = video.Trailer?.FilePath;
        var bannerPath = video.Banner?.Path;
        var thumbPath = video.Thumb?.Path;
        var thumbHalfPath = video.ThumbHalf?.Path;

        await _repository.Delete(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        await ClearStorage(
            mediaPath, trailerPath, bannerPath,
            thumbPath, thumbHalfPath, cancellationToken
        );
    }

    private async Task ClearStorage(
        string? mediaPath,
        string? trailerPath,
        string? bannerPath,
        string? thumbFilePath,
        string? thumbHalfPath,
        CancellationToken cancellationToken
    )
    {
        if (mediaPath is not null)
            await _storageService.Delete(mediaPath, cancellationToken);
        if (trailerPath is not null)
            await _storageService.Delete(trailerPath, cancellationToken);
        if (thumbFilePath is not null)
            await _storageService.Delete(thumbFilePath, cancellationToken);
        if (thumbHalfPath is not null)
            await _storageService.Delete(thumbHalfPath, cancellationToken);
        if (bannerPath is not null)
            await _storageService.Delete(bannerPath, cancellationToken);
    }
}
