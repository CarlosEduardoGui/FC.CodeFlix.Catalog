using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private readonly VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture videoTestFixture)
        => _fixture = videoTestFixture;

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(InstantiateOk))]
    public void InstantiateOk()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedRating = _fixture.GetRandomRating();

        var video = new DomainEntity.Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration,
            expectedRating
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
        video.Rating.Should().Be(expectedRating);
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
        video.Categories.Should().BeEmpty();
        video.Genres.Should().BeEmpty();
        video.CastMembers.Should().BeEmpty();
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(Update))]
    public void Update()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var video = _fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateWithRating))]
    public void UpdateWithRating()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedRating = _fixture.GetRandomRating();
        var video = _fixture.GetValidVideo();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration,
            expectedRating
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
        video.Rating.Should().Be(expectedRating);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    public void ValidateWhenValidState()
    {
        var video = _fixture.GetValidVideo();
        var notificationHandler = new NotificationValidationHandler();

        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
        notificationHandler.Errors.Should().HaveCount(0);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(InvalidWhenHasErrors))]
    public void InvalidWhenHasErrors()
    {
        var invalidVideo = new DomainEntity.Video(
            _fixture.GetTooLongTitle(),
            _fixture.GetTooLongDescription(),
            _fixture.GetValidYearLauched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationHandler = new NotificationValidationHandler();

        invalidVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
        {
            new("Title should be less or equal 255 characters long."),
            new("Description should be less or equal 4000 characters long.")
        });
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(ValidateWhenVideoUpdateStillValidEntity))]
    public void ValidateWhenVideoUpdateStillValidEntity()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var video = _fixture.GetValidVideo();
        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );
        var notificationHandler = new NotificationValidationHandler();

        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
        notificationHandler.Errors.Should().HaveCount(0);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(InvalidateWhenVideoUpdateIsNotValidEntity))]
    public void InvalidateWhenVideoUpdateIsNotValidEntity()
    {
        var expectedTitle = _fixture.GetTooLongTitle();
        var expectedDescription = _fixture.GetTooLongDescription();
        var expectedYearLaunched = _fixture.GetValidYearLauched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();
        var video = _fixture.GetValidVideo();
        video.Update(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );
        var notificationHandler = new NotificationValidationHandler();

        video.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
        {
            new("Title should be less or equal 255 characters long."),
            new("Description should be less or equal 4000 characters long.")
        });
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateThumb))]
    public void UpdateThumb()
    {
        var video = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        video.UpdateThumb(validImagePath);

        video.Thumb.Should().NotBeNull();
        video.Thumb!.Path.Should().Be(validImagePath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    public void UpdateThumbHalf()
    {
        var video = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        video.UpdateThumbHalf(validImagePath);

        video.ThumbHalf.Should().NotBeNull();
        video.ThumbHalf!.Path.Should().Be(validImagePath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateBanner))]
    public void UpdateBanner()
    {
        var video = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        video.UpdateBanner(validImagePath);

        video.Banner.Should().NotBeNull();
        video.Banner!.Path.Should().Be(validImagePath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateMedia))]
    public void UpdateMedia()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();

        video.UpdateMedia(validMediaPath);

        video.Media.Should().NotBeNull();
        video.Media!.FilePath.Should().Be(validMediaPath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateTrailer))]
    public void UpdateTrailer()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();

        video.UpdateTrailer(validMediaPath);

        video.Trailer.Should().NotBeNull();
        video.Trailer!.FilePath.Should().Be(validMediaPath);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    public void UpdateAsSentToEncode()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();
        video.UpdateMedia(validMediaPath);

        video.UpdateAsSentToEncode();

        video.Media.Should().NotBeNull();
        video.Media!.Status.Should().Be(MediaStatus.Processing);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
    public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
    {
        var video = _fixture.GetValidVideo();

        var action = () => video.UpdateAsSentToEncode();

        action
            .Should()
            .ThrowExactly<EntityValidationException>()
            .WithMessage("Media should not be null.");
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    public void UpdateAsEncoded()
    {
        var video = _fixture.GetValidVideo();
        var validMediaPath = _fixture.GetValidMediaPath();
        video.UpdateMedia(validMediaPath);
        var validEncodedPath = _fixture.GetValidMediaPath();

        video.UpdateAsEncoded(validEncodedPath);

        video.Media.Should().NotBeNull();
        video.Media!.EncodedPath.Should().Be(validEncodedPath);
        video.Media.Status.Should().Be(MediaStatus.Completed);
    }


    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(UpdateAsEncodedThrowsThereIsNoMedia))]
    public void UpdateAsEncodedThrowsThereIsNoMedia()
    {
        var video = _fixture.GetValidVideo();
        var validEncodedPath = _fixture.GetValidMediaPath();

        var action = () => video.UpdateAsEncoded(validEncodedPath);

        action
            .Should()
            .ThrowExactly<EntityValidationException>()
            .WithMessage("Media should not be null.");
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(AddCategoryValid))]
    public void AddCategoryValid()
    {
        var video = _fixture.GetValidVideo();
        var categoryId = Guid.NewGuid();

        video.AddCategory(categoryId);

        video.Should().NotBeNull();
        video.Categories.Should().HaveCount(1);
        video.Categories[0].Should().Be(categoryId);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(RemoveCategory))]
    public void RemoveCategory()
    {
        var video = _fixture.GetValidVideo();
        var categoryIdOne = Guid.NewGuid();
        var categoryIdTwo = Guid.NewGuid();
        video.AddCategory(categoryIdOne);
        video.AddCategory(categoryIdTwo);

        video.RemoveCategory(categoryIdOne);

        video.Should().NotBeNull();
        video.Categories.Should().HaveCount(1);
        video.Categories[0].Should().Be(categoryIdTwo);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(RemoveAllCategories))]
    public void RemoveAllCategories()
    {
        var video = _fixture.GetValidVideo();
        var categoryIdOne = Guid.NewGuid();
        var categoryIdTwo = Guid.NewGuid();
        video.AddCategory(categoryIdOne);
        video.AddCategory(categoryIdTwo);

        video.RemoveAllCategories();

        video.Should().NotBeNull();
        video.Categories.Should().BeEmpty();
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(AddGenreValid))]
    public void AddGenreValid()
    {
        var video = _fixture.GetValidVideo();
        var genreId = Guid.NewGuid();

        video.AddGenre(genreId);

        video.Should().NotBeNull();
        video.Genres.Should().HaveCount(1);
        video.Genres[0].Should().Be(genreId);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(RemoveGenre))]
    public void RemoveGenre()
    {
        var video = _fixture.GetValidVideo();
        var genreIdOne = Guid.NewGuid();
        var genreIdTwo = Guid.NewGuid();
        video.AddGenre(genreIdOne);
        video.AddGenre(genreIdTwo);

        video.RemoveGenre(genreIdOne);

        video.Should().NotBeNull();
        video.Genres.Should().HaveCount(1);
        video.Genres[0].Should().Be(genreIdTwo);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(RemoveAllGenres))]
    public void RemoveAllGenres()
    {
        var video = _fixture.GetValidVideo();
        var genreIdOne = Guid.NewGuid();
        var genreIdTwo = Guid.NewGuid();
        video.AddGenre(genreIdOne);
        video.AddGenre(genreIdTwo);

        video.RemoveAllGenres();

        video.Should().NotBeNull();
        video.Genres.Should().BeEmpty();
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(AddCastMemberValid))]
    public void AddCastMemberValid()
    {
        var video = _fixture.GetValidVideo();
        var castMemberId = Guid.NewGuid();

        video.AddCastMember(castMemberId);

        video.Should().NotBeNull();
        video.CastMembers.Should().HaveCount(1);
        video.CastMembers[0].Should().Be(castMemberId);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(RemoveCastMember))]
    public void RemoveCastMember()
    {
        var video = _fixture.GetValidVideo();
        var castMemberIdOne = Guid.NewGuid();
        var castMemberIdTwo = Guid.NewGuid();
        video.AddCastMember(castMemberIdOne);
        video.AddCastMember(castMemberIdTwo);

        video.RemoveCastMember(castMemberIdOne);

        video.Should().NotBeNull();
        video.CastMembers.Should().HaveCount(1);
        video.CastMembers[0].Should().Be(castMemberIdTwo);
    }

    [Trait("Domain", "Video - Aggregate")]
    [Fact(DisplayName = nameof(RemoveAllCastMember))]
    public void RemoveAllCastMember()
    {
        var video = _fixture.GetValidVideo();
        var castMemberIdOne = Guid.NewGuid();
        var castMemberIdTwo = Guid.NewGuid();
        video.AddCastMember(castMemberIdOne);
        video.AddCastMember(castMemberIdTwo);

        video.RemoveAllCastMembers();

        video.Should().NotBeNull();
        video.CastMembers.Should().BeEmpty();
    }
}
