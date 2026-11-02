using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validator;
using FC.Codeflix.Catalog.Domain.ValueObject;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Video(
    string title,
    string description,
    bool opened,
    bool published,
    int yearLaunched,
    int duration,
    ERating rating,

    Image? thumb = null,
    Image? thumbHalf = null,
    Image? banner = null
) : AggregateRoot
{
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
    public bool Opened { get; private set; } = opened;
    public bool Published { get; private set; } = published;
    public int YearLaunched { get; private set; } = yearLaunched;
    public int Duration { get; private set; } = duration;
    public ERating Rating { get; private set; } = rating;
    public Image? Thumb { get; private set; } = thumb;
    public Image? ThumbHalf { get; private set; } = thumbHalf;
    public Image? Banner { get; private set; } = banner;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public void Update(
        string? title = null,
        string? description = null,
        bool? opened = null,
        bool? published = null,
        int? yearLaunched = null,
        int? duration = null,
        ERating? rating = null
    )
    {
        Title = title ?? Title;
        Description = description ?? Description;
        Opened = opened ?? Opened;
        Published = published ?? Published;
        YearLaunched = yearLaunched ?? YearLaunched;
        Duration = duration ?? Duration;
        Rating = rating ?? Rating;
    }

    public void Validate()
        => new VideoValidator(this, Notification).Validate();

    public void UpdateThumb(string path)
        => Thumb = new Image(path);

    public void UpdateThumbHalf(string path)
        => ThumbHalf = new Image(path);

    public void UpdateBanner(string path)
        => Banner = new Image(path);
}
