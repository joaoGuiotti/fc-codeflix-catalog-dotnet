using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo(
    IVideoRepository repo,
    ICategoryRepository categoryRepo,
    IGenreRepository genreRepo,
    ICastMemberRepository castMemberRepo,
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

        await repo.Insert(video, cancellationToken);
        await uow.Commit(cancellationToken);

        return CreateVideoOutput.FromEntity(video);
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
