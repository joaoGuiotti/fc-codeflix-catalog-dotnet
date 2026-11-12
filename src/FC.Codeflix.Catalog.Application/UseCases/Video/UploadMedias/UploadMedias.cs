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
            await UploadImages(request, video, cancellationToken);

            await _repo.Update(video, cancellationToken);
            await _uow.Commit(cancellationToken);
        }
        catch (Exception e)
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
        if (request.BannerFile is not null && video.Banner is not null)
            await _storageService.Delete(video.Banner.Path, cancellationToken);
        if (request.ThumbFile is not null && video.Thumb is not null)
            await _storageService.Delete(video.Thumb.Path, cancellationToken);
        if (request.ThumbHalfFile is not null && video.ThumbHalf is not null)
            await _storageService.Delete(video.ThumbHalf.Path, cancellationToken);
    }

    private async Task UploadImages(
        UploadMediasInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.BannerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), request.BannerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                request.BannerFile.FileStream,
                request.BannerFile.ContentType,
                cancellationToken
            );
            video.UpdateBanner(uploadedFilePath);
        }

        if (request.ThumbFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), request.ThumbFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                request.ThumbFile.FileStream,
                request.ThumbFile.ContentType,
                cancellationToken
            );
            video.UpdateThumb(uploadedFilePath);
        }

        if (request.ThumbHalfFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), request.ThumbHalfFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                request.ThumbHalfFile.FileStream,
                request.ThumbHalfFile.ContentType,
                cancellationToken
            );
            video.UpdateThumbHalf(uploadedFilePath);
        }
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
