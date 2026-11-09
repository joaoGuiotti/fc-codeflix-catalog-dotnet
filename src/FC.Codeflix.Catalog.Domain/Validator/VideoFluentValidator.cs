using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentValidation;

namespace FC.Codeflix.Catalog.Domain.Validator;

public class VideoFluentValidator : AbstractValidator<Video>
{
    public VideoFluentValidator()
    {
        RuleFor(video => video.Description)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty().WithMessage($"{nameof(Video.Description)} is required")
            .Length(3, 4_000).WithMessage($"{nameof(Video.Description)} must be between 3 and 4.000 characters");

        RuleFor(video => video.Title)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty().WithMessage($"{nameof(Video.Title)} is required")
            .Length(3, 255).WithMessage($"{nameof(Video.Title)} must be between 3 and 255 characters");
    }
}
