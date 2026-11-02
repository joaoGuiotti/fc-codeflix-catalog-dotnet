using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validator;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Video(
    string title,
    string description,
    bool opened,
    bool published,
    int yearLaunched,
    int duration,
    ERating rating
) : AggregateRoot
{
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public bool Opened { get; set; } = opened;
    public bool Published { get; set; } = published;
    public int YearLaunched { get; set; } = yearLaunched;
    public int Duration { get; set; } = duration;
    public ERating Rating { get; set; } = rating;
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
}
