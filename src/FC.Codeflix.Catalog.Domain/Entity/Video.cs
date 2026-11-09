using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using FC.Codeflix.Catalog.Domain.ValueObject;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Video : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public ERating Rating { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public Media? Trailer { get; private set; }
    public Media? Media { get; private set; }

    private readonly List<Guid> _categories = new List<Guid>();
    public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();

    private readonly List<Guid> _genres = new List<Guid>();
    public IReadOnlyList<Guid> Genres => _genres.AsReadOnly();

    private readonly List<Guid> _castMembers = new List<Guid>();

    public Video(
        string title,
        string description,
        bool opened,
        bool published,
        int yearLaunched,
        int duration,
        ERating rating
    )
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
        Rating = rating;

        Validate();
    }

    public IReadOnlyList<Guid> CastMembers => _castMembers.AsReadOnly();

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

        Validate();
    }

    public void Validate()
        => new VideoValidator(this, Notification).Validate();

    public void UpdateThumb(string path)
        => Thumb = new Image(path);

    public void UpdateThumbHalf(string path)
        => ThumbHalf = new Image(path);

    public void UpdateBanner(string path)
        => Banner = new Image(path);

    public void UpdateMedia(string path)
        => Media = new Media(path);

    public void UpdateTrailer(string path)
        => Trailer = new Media(path);

    public void UpdateAsSendToEncode()
    {
        if (Media is null)
        {
            Notification.AddError($"{nameof(Video)} there is no Media!");
            return;
        }

        Media.UpdateAsSentToEncode();
    }

    public void UpdateAsEncoded(string encodedPath)
    {
        if (Media is null)
        {
            Notification.AddError($"{nameof(Video)} there is no Media!");
            return;
        }

        Media.UpdateAsEnconded(encodedPath);
    }

    public void AddCategory(Guid categoryId)
        => _categories.Add(categoryId);

    public void RemoveCategory(Guid categoryId)
        => _categories.Remove(categoryId);

    public void RemoveAllCategories()
        => _categories.Clear();

    public void AddGenre(Guid genreId)
        => _genres.Add(genreId);

    public void RemoveGenre(Guid genreId)
        => _genres.Remove(genreId);

    public void RemoveAllGenres()
        => _genres.Clear();

    public void AddCastMember(Guid castMemberId)
        => _castMembers.Add(castMemberId);

    public void RemoveCastMember(Guid castMemberId)
        => _castMembers.Remove(castMemberId);

    public void RemoveAllCastMembers()
        => _castMembers.Clear();

    public void RemoveAllCategory()
    => _categories.Clear();
}
