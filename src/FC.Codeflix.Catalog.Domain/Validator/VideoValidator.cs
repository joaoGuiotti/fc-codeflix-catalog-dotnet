using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Validator;

public class VideoValidator(Video video, INotification notification) : Validation.Validator(notification)
{
    public override void Validate()
    {
        var result = new VideoFluentValidator().Validate(video);

        foreach (var error in result.Errors)
            Notification.AddError(error.ErrorMessage, error.PropertyName);
    }
}
