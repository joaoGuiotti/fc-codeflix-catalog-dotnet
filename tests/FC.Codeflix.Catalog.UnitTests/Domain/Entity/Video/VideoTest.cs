using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private readonly VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InstantiateVideo))]
    [Trait("Domain", "Video - Aggregate")]
    public void InstantiateVideo()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedRating = _fixture.GetRandomRating();

        var dateTimeBefore = DateTime.Now;
        var video = new DomainEntity.Video(
            expectedTitle,
            expectedDescription,
            expectedOpened,
            expectedPublished,
            expectedYearLaunched,
            expectedDuration,
            expectedRating
        );
        var dateTimeAfter = DateTime.Now;

        video.Should().NotBeNull();
        video.Id.Should().NotBeEmpty();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
        video.Rating.Should().Be(expectedRating);
        video.CreatedAt.Should().NotBeSameDateAs(default);
        video.CreatedAt.Should().BeOnOrAfter(dateTimeBefore);
        video.CreatedAt.Should().BeOnOrBefore(dateTimeAfter);
        video.Notification.HasErrors.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsTooLong()
    {
        var invalidVideo = _fixture.GetValidVideo(
            description: _fixture.GetTooLongDescription()
        );

        invalidVideo.Validate();

        invalidVideo.Notification.Errors.Should().HaveCount(1);
        invalidVideo.Notification.Errors.First()
            .Message.Should().Be("Description must be between 3 and 255 characters");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsEmpty()
    {
        var invalidVideo = _fixture.GetValidVideo(
            title: ""
        );

        invalidVideo.Validate();

        invalidVideo.Notification.Errors.Should().NotBeNull();
        invalidVideo.Notification.Errors.Should().HaveCount(1);
        invalidVideo.Notification.Errors.First()
            .Message.Should().Be("Title is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorsWhenIsInvalidState))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorsWhenIsInvalidState()
    {
        var invalidVideo = _fixture.GetValidVideo(
            title: "",
            description: ""
        );

        invalidVideo.Validate();

        invalidVideo.Notification.Errors.Should().NotBeNull();
        invalidVideo.Notification.Errors.Should().HaveCount(2);
        invalidVideo.Notification.Errors.Should()
            .BeEquivalentTo([
                new ValidationError($"{nameof(invalidVideo.Title)} is required"),
                new ValidationError($"{nameof(invalidVideo.Description)} is required")
            ]);
    }

    [Fact(DisplayName = nameof(UpdateValidVideoToInvalid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void UpdateValidVideoToInvalid()
    {
        var video = _fixture.GetValidVideo();

        video.Notification.HasErrors.Should().BeFalse();

        video.Update(
            title: _fixture.GetTooLongTitle()
        );

        video.Validate();

        video.Notification.Errors.Should().NotBeNull();
        video.Notification.Errors.Should().HaveCount(1);
        video.Notification.Errors.First()
            .Message.Should().Be($"{nameof(video.Title)} must be between 3 and 255 characters");
    }

    [Fact(DisplayName = nameof(UpdateValidVideoStillValid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void UpdateValidVideoStillValid()
    {
        var video = _fixture.GetValidVideo();

        video.Notification.HasErrors.Should().BeFalse();

        var title = _fixture.GetValidTitle();
        var description = _fixture.GetValidDescription();
        var published = _fixture.GetRandomBoolean();
        var rating = _fixture.GetRandomRating();

        video.Update(
            title: title,
            description: description,
            published: published,
            rating: rating
        );

        video.Validate();

        video.Notification.Errors.Should().NotBeNull();
        video.Notification.Errors.Should().HaveCount(0);
        video.Title.Should().Be(title);
        video.Description.Should().Be(description);
        video.Published.Should().Be(published);
        video.Rating.Should().Be(rating);
    }

    [Fact(DisplayName = nameof(UpdateThumb))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateThumb()
    {
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        validVideo.UpdateThumb(validImagePath);

        validVideo.Thumb.Should().NotBeNull();
        validVideo.Thumb!.Path.Should().Be(validImagePath);
        validVideo.Thumb.Should().Be(new Image(validImagePath));
    }

    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateThumbHalf()
    {
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        validVideo.UpdateThumbHalf(validImagePath);

        validVideo.ThumbHalf.Should().NotBeNull();
        validVideo.ThumbHalf!.Path.Should().Be(validImagePath);
        validVideo.ThumbHalf.Should().Be(new Image(validImagePath));
    }

    [Fact(DisplayName = nameof(UpdateBanner))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateBanner()
    {
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        validVideo.UpdateBanner(validImagePath);

        validVideo.Banner.Should().NotBeNull();
        validVideo.Banner!.Path.Should().Be(validImagePath);
        validVideo.Banner.Should().Be(new Image(validImagePath));
    }
}
