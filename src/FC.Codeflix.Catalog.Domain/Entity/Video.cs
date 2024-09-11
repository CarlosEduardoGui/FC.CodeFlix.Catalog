using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using FC.Codeflix.Catalog.Domain.ValueObject;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Video : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int YearLaunched { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int Duration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Rating Rating { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public Media? Media { get; private set; }
    public Media? Trailer { get; private set; }
    public List<Guid> _categories;
    public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();
    public List<Guid> _genres;
    public IReadOnlyList<Guid> Genres =>
        _genres.AsReadOnly();
    public List<Guid> _castMembers;
    public IReadOnlyList<Guid> CastMembers =>
        _castMembers.AsReadOnly();

    public Video(
        string title,
        string description,
        int yearLaunched,
        bool opened,
        bool published,
        int duration,
        Rating rating
    )
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Opened = opened;
        Published = published;
        Duration = duration;
        Rating = rating;
        CreatedAt = DateTime.Now;

        _categories = new List<Guid>();
        _genres = new List<Guid>();
        _castMembers = new List<Guid>();
    }

    public void Validate(ValidationHandler handler)
        => new VideoValidator(this, handler).Validate();

    public void Update(
        string title,
        string description,
        int yearLaunched,
        bool opened,
        bool published,
        int duration,
        Rating? rating = null
    )
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Opened = opened;
        Published = published;
        Duration = duration;
        if(rating is not null)
            Rating = rating.Value;
    }

    public void UpdateThumb(string imagePath)
        => Thumb = new(imagePath);

    public void UpdateThumbHalf(string imagePath)
        => ThumbHalf = new(imagePath);

    public void UpdateBanner(string imagePath)
        => Banner = new(imagePath);

    public void UpdateMedia(string mediaPath)
        => Media = new Media(mediaPath);

    public void UpdateTrailer(string mediaPath)
        => Trailer = new(mediaPath);

    public void UpdateAsSentToEncode()
    {
        if (Media is null)
            throw new EntityValidationException(
                string.Format(ConstantsMessages.FIELD_NOT_NULL, nameof(Media))
            );

        Media!.UpdateAsSentToEncode();
    }

    public void UpdateAsEncoded(string encodedPath)
    {
        if (Media is null)
            throw new EntityValidationException(
                string.Format(ConstantsMessages.FIELD_NOT_NULL, nameof(Media))
            );

        Media.UpdateAsEncoded(encodedPath);
    }

    public void AddCategory(Guid categoryId)
        => _categories.Add(categoryId);

    public void RemoveCategory(Guid categoryIdOne)
        => _categories.Remove(categoryIdOne);

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
}
