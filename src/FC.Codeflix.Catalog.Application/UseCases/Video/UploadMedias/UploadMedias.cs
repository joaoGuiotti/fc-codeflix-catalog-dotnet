using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;

public class UploadMedias : IUploadMedias
{
    private readonly IVideoRepository _repo;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _uow;

    public UploadMedias(
        IVideoRepository repo,
        IStorageService storageService,
        IUnitOfWork uow
    )
    {
        _repo = repo;
        _storageService = storageService;
        _uow = uow;
    }

    public async Task Handle(
        UploadMediasInput request,
        CancellationToken cancellationToken
    )
    {
        var video = await _repo.Get(request.VideoId, cancellationToken);

        try
        {
            await UploadVideo(request, video, cancellationToken);
            await UploadTrailer(request, video, cancellationToken);

            await _repo.Update(video, cancellationToken);
            await _uow.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(request, video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(
        UploadMediasInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.VideoFile is not null && video.Media is not null)
            await _storageService.Delete(video.Media.FilePath, cancellationToken);
        if (request.TrailerFile is not null && video.Trailer is not null)
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadTrailer(
        UploadMediasInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), request.TrailerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                request.TrailerFile.FileStream,
                request.TrailerFile.ContentType,
                cancellationToken
            );
            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private async Task UploadVideo(
        UploadMediasInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.VideoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), request.VideoFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                request.VideoFile.FileStream,
                request.VideoFile.ContentType,
                cancellationToken
            );
            video.UpdateMedia(uploadedFilePath);
        }
    }
}
