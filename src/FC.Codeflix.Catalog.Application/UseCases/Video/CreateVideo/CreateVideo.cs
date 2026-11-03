using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo(
    IVideoRepository repo,
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
            throw new EntityValidationException("There are validation errors.",  notification.Errors);

        await repo.Insert(video, cancellationToken);
        await uow.Commit(cancellationToken);

        return CreateVideoOutput.FromEntity(video);
    }
}
