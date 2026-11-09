using FC.Codeflix.Catalog.Domain.Enum;
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

        video.Notification.HasErrors().Should().BeFalse();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
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
            .Should().Be(new ValidationError("Description must be between 3 and 4.000 characters", "Description"));
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
            .Should().Be(new ValidationError("Title is required", "Title"));
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
                new ValidationError($"{nameof(invalidVideo.Title)} is required", nameof(invalidVideo.Title)),
                new ValidationError($"{nameof(invalidVideo.Description)} is required", nameof(invalidVideo.Description))
            ]);
    }

    [Fact(DisplayName = nameof(UpdateValidVideoToInvalid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void UpdateValidVideoToInvalid()
    {
        var video = _fixture.GetValidVideo();

        video.Notification.HasErrors().Should().BeFalse();

        video.Update(
            title: _fixture.GetTooLongTitle()
        );

        video.Validate();

        video.Notification.Errors.Should().NotBeNull();
        video.Notification.Errors.Should().HaveCount(1);
        video.Notification.Errors.First()
            .Should().Be(new ValidationError("Title must be between 3 and 255 characters", "Title"));
    }

    [Fact(DisplayName = nameof(UpdateValidVideoStillValid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void UpdateValidVideoStillValid()
    {
        var video = _fixture.GetValidVideo();

        video.Notification.HasErrors().Should().BeFalse();

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

    [Fact(DisplayName = nameof(UpdateMedia))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateMedia()
    {
        var video = _fixture.GetValidVideo();
        var mediaPath = _fixture.GetValidMediaPath();

        video.UpdateMedia(mediaPath);

        video.Media.Should().NotBeNull();
        video.Media.Status.Should().Be(EMediaStatus.Pending);
        video.Media.FilePath.Should().Be(mediaPath);
    }

    [Fact(DisplayName = nameof(UpdateTrailer))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateTrailer()
    {
        var video = _fixture.GetValidVideo();
        var mediaPath = _fixture.GetValidMediaPath();

        video.UpdateTrailer(mediaPath);

        video.Trailer.Should().NotBeNull();
        video.Trailer.Status.Should().Be(EMediaStatus.Pending);
        video.Trailer.FilePath.Should().Be(mediaPath);
    }

    [Fact(DisplayName = nameof(UpdateAsSendToEncode))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateAsSendToEncode()
    {
        var video = _fixture.GetValidVideo();
        var mediaPath = _fixture.GetValidMediaPath();

        video.UpdateMedia(mediaPath);
        video.UpdateAsSendToEncode();

        video.Media.Should().NotBeNull();
        video.Media.Status.Should().Be(EMediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateAsEncoded()
    {
        var video = _fixture.GetValidVideo();
        var mediaPath = _fixture.GetValidMediaPath();
        var encodedPath = _fixture.GetValidMediaPath();

        video.UpdateMedia(mediaPath);
        video.UpdateAsEncoded(encodedPath);

        video.Media.Should().NotBeNull();
        video.Media.FilePath.Should().Be(mediaPath);
        video.Media.EncodedPath.Should().Be(encodedPath);
        video.Media.Status.Should().Be(EMediaStatus.Completed);
    }

    [Fact(DisplayName = nameof(UpdateAsEncodedThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateAsEncodedThrowsWhenThereIsNoMedia()
    {
        var video = _fixture.GetValidVideo();
        var encodedPath = _fixture.GetValidMediaPath();

        video.UpdateAsEncoded(encodedPath);

        video.Notification.HasErrors().Should().BeTrue();
        video.Notification.Errors.First()
            .Should().Be(new ValidationError("Video there is no Media!"));
    }

    [Fact(DisplayName = nameof(UpdateAsSendToEncodeThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateAsSendToEncodeThrowsWhenThereIsNoMedia()
    {
        var video = _fixture.GetValidVideo();
        var encodedPath = _fixture.GetValidMediaPath();

        video.UpdateAsSendToEncode();

        video.Notification.HasErrors().Should().BeTrue();
        video.Notification.Errors.First()
            .Should().Be(new ValidationError("Video there is no Media!"));
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Video - Aggregate")]
    public void AddCategory()
    {
        var video = _fixture.GetValidVideo();
        var categoryId = Guid.NewGuid();

        video.AddCategory(categoryId);

        video.Categories.Should().HaveCount(1);
        video.Categories.First().Should().Be(categoryId);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveCategory()
    {
        var video = _fixture.GetValidVideo();
        var categoryId = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();

        video.AddCategory(categoryId);
        video.AddCategory(categoryId2);

        video.RemoveCategory(categoryId);

        video.Categories.Should().HaveCount(1);
        video.Categories.Should().NotContain(categoryId);
    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveAllCategories()
    {
        var video = _fixture.GetValidVideo();
        var categoryId = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();

        video.AddCategory(categoryId);
        video.AddCategory(categoryId2);

        video.RemoveAllCategories();

        video.Categories.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(AddGenre))]
    [Trait("Domain", "Video - Aggregate")]
    public void AddGenre()
    {
        var video = _fixture.GetValidVideo();
        var genreId = Guid.NewGuid();

        video.AddGenre(genreId);

        video.Genres.Should().HaveCount(1);
        video.Genres.First().Should().Be(genreId);
    }

    [Fact(DisplayName = nameof(RemoveGenre))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveGenre()
    {
        var video = _fixture.GetValidVideo();
        var genreId = Guid.NewGuid();
        var genreId2 = Guid.NewGuid();

        video.AddGenre(genreId);
        video.AddGenre(genreId2);

        video.RemoveGenre(genreId2);

        video.Genres.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(RemoveAllGenres))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveAllGenres()
    {
        var video = _fixture.GetValidVideo();
        var genreId = Guid.NewGuid();
        var genreId2 = Guid.NewGuid();

        video.AddGenre(genreId);
        video.AddGenre(genreId2);

        video.RemoveAllGenres();

        video.Genres.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(AddCastMember))]
    [Trait("Domain", "Video - Aggregate")]
    public void AddCastMember()
    {
        var video = _fixture.GetValidVideo();
        var castMemberId = Guid.NewGuid();

        video.AddCastMember(castMemberId);

        video.CastMembers.Should().HaveCount(1);
        video.CastMembers.First().Should().Be(castMemberId);
    }

    [Fact(DisplayName = nameof(RemoveCastMember))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveCastMember()
    {
        var video = _fixture.GetValidVideo();
        var castMemberId = Guid.NewGuid();
        var castMemberId2 = Guid.NewGuid();

        video.AddCastMember(castMemberId);
        video.AddCastMember(castMemberId2);

        video.RemoveCastMember(castMemberId2);

        video.CastMembers.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(RemoveAllCastMembers))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveAllCastMembers()
    {
        var video = _fixture.GetValidVideo();
        var castMemberId = Guid.NewGuid();
        var castMemberId2 = Guid.NewGuid();

        video.AddCastMember(castMemberId);
        video.AddCastMember(castMemberId2);

        video.RemoveAllCastMembers();

        video.CastMembers.Should().HaveCount(0);
    }
}
