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

        await _repository.Delete(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        await ClearStorage(video, cancellationToken);
    }

    private async Task ClearStorage(
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (video.Media is not null)
            await _storageService.Delete(video.Media.FilePath, cancellationToken);
        if (video.Trailer is not null)
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }
}
