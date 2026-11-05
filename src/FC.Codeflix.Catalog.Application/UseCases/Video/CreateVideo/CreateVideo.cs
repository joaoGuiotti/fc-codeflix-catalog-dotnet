using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo(
    IVideoRepository repo,
    ICategoryRepository categoryRepo,
    IGenreRepository genreRepo,
    ICastMemberRepository castMemberRepo,
    IStorageService storageService,
    IUnitOfWork uow
) : ICreateVideo
{
    public async Task<CreateVideoOutput> Handle(
        CreateVideoInput request,
        CancellationToken cancellationToken
    )
    {
        var video = new Domain.Entity.Video(
            request.Title,
            request.Description,
            request.Opened,
            request.Published,
            request.YearLaunched,
            request.Duration,
            request.Rating
        );

        var notification = video.Notification;

        if (notification.HasErrors())
            throw new EntityValidationException("There are validation errors.", notification.Errors);

        await ValidateAndAddRelations(request, video, cancellationToken);

        await TryUploadMedias(request, video, cancellationToken);

        return CreateVideoOutput.FromEntity(video);
    }

    private async Task TryUploadMedias(
        CreateVideoInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await UploadImagesMedias(request, video, cancellationToken);
            await UploadVideosMedias(request, video, cancellationToken);

            await repo.Insert(video, cancellationToken);
            await uow.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorageMedias(cancellationToken, video);
            throw;
        }
    }

    private async Task UploadVideosMedias(
        CreateVideoInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.Media is not null)
        {
            var mediaUrl = await storageService.Upload(
                StorageFileName.Create(video.Id, nameof(video.Media), request.Media.Extension),
                request.Media.FileStream,
                cancellationToken
            );
            video.UpdateMedia(mediaUrl);
        }

        if (request.Trailer is not null)
        {
            var trailerUrl = await storageService.Upload(
                StorageFileName.Create(video.Id, nameof(video.Trailer), request.Trailer.Extension),
                request.Trailer.FileStream,
                cancellationToken
            );
            video.UpdateTrailer(trailerUrl);
        }
    }

    private async Task ClearStorageMedias(CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (video.Thumb is not null)
            await storageService.Delete(video.Thumb.Path, cancellationToken);
        if (video.ThumbHalf is not null)
            await storageService.Delete(video.ThumbHalf.Path, cancellationToken);
        if (video.Banner is not null)
            await storageService.Delete(video.Banner.Path, cancellationToken);
        if (video.Media is not null)
            await storageService.Delete(video.Media.FilePath, cancellationToken);
        if (video.Trailer is not null)
            await storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadImagesMedias(
        CreateVideoInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.Thumb is not null)
        {
            var thumbUrl = await storageService.Upload(
                StorageFileName.Create(video.Id, nameof(video.Thumb), request.Thumb.Extension),
                request.Thumb.FileStream,
                cancellationToken
            );

            video.UpdateThumb(thumbUrl);
        }

        if (request.Banner is not null)
        {
            var bannerUrl = await storageService.Upload(
                StorageFileName.Create(video.Id, nameof(video.Banner), request.Banner.Extension),
                request.Banner.FileStream,
                cancellationToken
            );

            video.UpdateBanner(bannerUrl);
        }

        if (request.ThumbHalf is not null)
        {
            var thumbHalfUrl = await storageService.Upload(
                StorageFileName.Create(video.Id, nameof(video.ThumbHalf), request.ThumbHalf.Extension),
                request.ThumbHalf.FileStream,
                cancellationToken
            );

            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private async Task ValidateAndAddRelations(
        CreateVideoInput request,
        Domain.Entity.Video video,
        CancellationToken cancellationToken
    )
    {
        if (request.CategoriesIds?.Count > 0)
        {
            await ValidateCategoriesIds(request.CategoriesIds, cancellationToken);
            request.CategoriesIds.ToList().ForEach(video.AddCategory);
        }

        if (request.GenresIds?.Count > 0)
        {
            await ValidateGenresIds(request.GenresIds, cancellationToken);
            request.GenresIds.ToList().ForEach(video.AddGenre);
        }

        if (request.CastMembersIds?.Count > 0)
        {
            await ValidateCastMembersIds(request.CastMembersIds, cancellationToken);
            request.CastMembersIds.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateCastMembersIds(
        List<Guid> requestCastMembersIds,
        CancellationToken cancellationToken
    )
    {
        var idsInPersistence = await castMemberRepo.GetIdsListById(
            requestCastMembersIds,
            cancellationToken
        );

        RelatedAggregateException.ThrowIfNotFound(
            requestCastMembersIds,
            idsInPersistence,
            nameof(Domain.Entity.CastMember)
        );
    }

    private async Task ValidateGenresIds(
        List<Guid> requestGenresIds,
        CancellationToken cancellationToken
    )
    {
        var idsInPersistence = await genreRepo.GetIdsListById(
            requestGenresIds,
            cancellationToken
        );

        RelatedAggregateException.ThrowIfNotFound(
            requestGenresIds,
            idsInPersistence,
            nameof(Domain.Entity.Genre)
        );
    }

    private async Task ValidateCategoriesIds(
        List<Guid> categoriesIds,
        CancellationToken cancellationToken
    )
    {
        var idsInPersistence = await categoryRepo.GetIdsListById(
            categoriesIds,
            cancellationToken
        );

        RelatedAggregateException.ThrowIfNotFound(
            categoriesIds,
            idsInPersistence,
            nameof(Domain.Entity.Category)
        );
    }
}
