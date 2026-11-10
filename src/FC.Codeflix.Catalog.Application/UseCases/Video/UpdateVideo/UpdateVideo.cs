using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadVideo;

public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository _repo;
    private readonly IGenreRepository _genreRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ICastMemberRepository _castMemberRepo;
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storageService;

    public UpdateVideo(
        IVideoRepository videoRepository,
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository,
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _repo = videoRepository;
        _genreRepo = genreRepository;
        _categoryRepo = categoryRepository;
        _castMemberRepo = castMemberRepository;
        _uow = unitOfWork;
        _storageService = storageService;
    }

    public async Task<VideoModelOutput> Handle(UpdateVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _repo.Get(request.Id, cancellationToken);

        video.Update(
            request.Title,
            request.Description,
            request.Opened,
            request.Published,
            request.YearLaunched,
            request.Duration,
            request.Rating
        );

        if (video.Notification.HasErrors())
            throw new EntityValidationException("There are validation errors in the entity", video.Notification.Errors);

        await ValidateAndAddRelations(request, video, cancellationToken);
        await UploadImagesMedia(video, request, cancellationToken);

        await _repo.Update(video, cancellationToken);
        await _uow.Commit(cancellationToken);

        return VideoModelOutput.FromEntity(video);
    }

    private async Task UploadImagesMedia(
       Domain.Entity.Video video,
       UpdateVideoInput input,
       CancellationToken cancellationToken)
    {
        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var bannerUrl = await _storageService.Upload(
            fileName,
                input.Banner.FileStream,
                input.Banner.ContentType,
                cancellationToken);
            video.UpdateBanner(bannerUrl);
        }

        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);
            var thumbUrl = await _storageService.Upload(
            fileName,
                input.Thumb.FileStream,
                input.Thumb.ContentType,
                cancellationToken);
            video.UpdateThumb(thumbUrl);
        }

        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbUrl = await _storageService.Upload(
            fileName,
                input.ThumbHalf.FileStream,
                input.ThumbHalf.ContentType,
                cancellationToken);
            video.UpdateThumbHalf(thumbUrl);
        }
    }

    private async Task ValidateAndAddRelations(
        UpdateVideoInput input,
        Domain.Entity.Video video,
        CancellationToken cancellationToken)
    {
        if (input.GenresIds?.Count > 0)
        {
            await ValidateGenresIds(input, cancellationToken);
            video.RemoveAllGenres();
            input.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        if (input.CategoriesIds?.Count > 0)
        {
            await ValidateCategoriesIds(input, cancellationToken);
            video.RemoveAllCategories();
            input.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }

        if (input.CastMembersIds?.Count > 0)
        {
            await ValidateCastMembersIds(input, cancellationToken);
            video.RemoveAllCastMembers();
            input.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateGenresIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _genreRepo.GetIdsListById(input.GenresIds!.ToList(), cancellationToken);
        RelatedAggregateException.ThrowIfNotFound(input.GenresIds!, persistenceIds, nameof(Genre));
    }

    private async Task ValidateCategoriesIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _categoryRepo.GetIdsListById(input.CategoriesIds!.ToList(), cancellationToken);
        RelatedAggregateException.ThrowIfNotFound(input.CategoriesIds!, persistenceIds, nameof(Category));
    }

    private async Task ValidateCastMembersIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _castMemberRepo.GetIdsListById(input.CastMembersIds!.ToList(), cancellationToken);
        RelatedAggregateException.ThrowIfNotFound(input.CastMembersIds!, persistenceIds, nameof(CastMember));

    }
}
