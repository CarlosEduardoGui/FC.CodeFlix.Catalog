using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Extension;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoOk))]
    public async Task CreateVideoOk()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    public async Task CreateVideoWithThumb()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidImageFileInput();
        var expectedThumbName = $"thumb.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(thumb: validFileInput);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        storageServicekMock.VerifyAll();
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    public async Task CreateVideoWithBanner()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidImageFileInput();
        var expectedThumbName = $"banner.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(banner: validFileInput);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        storageServicekMock.VerifyAll();
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.BannerFileUrl.Should().Be(expectedThumbName);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    public async Task CreateVideoWithThumbHalf()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidImageFileInput();
        var expectedThumbName = $"thumbhalf.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(thumbHalf: validFileInput);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        storageServicekMock.VerifyAll();
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating  .ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbName);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
    public async Task CreateVideoWithAllImages()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidImageFileInput();
        var expectedBannerName = $"banner.{validFileInput.Extension}";
        var expectedThumbName = $"thumb.{validFileInput.Extension}";
        var expectedThumbHalfName = $"thumbhalf.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains("-banner")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedBannerName);
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains("-thumb")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains("-thumbhalf")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbHalfName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInputWithAllImages();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        storageServicekMock.VerifyAll();
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.BannerFileUrl.Should().Be(expectedBannerName);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    public async Task CreateVideoWithMedia()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var expectedMediaName = $"/storage/{_fixture.GetValidMediaPath()}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedMediaName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(media: _fixture.GetValidMediaFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        storageServicekMock.VerifyAll();
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.VideoFileUrl.Should().Be(expectedMediaName);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    public async Task CreateVideoWithTrailer()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var expectedTrailerName = $"/storage/{_fixture.GetValidMediaPath()}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedTrailerName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(trailer: _fixture.GetValidMediaFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        storageServicekMock.VerifyAll();
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.TrailerFileUrl.Should().Be(expectedTrailerName);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsExceptionInImageUploadedErrorCase))]
    public async Task ThrowsExceptionInImageUploadedErrorCase()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidImageFileInput();
        var expectedBannerName = $"banner.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new Exception("something went wrong."));
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInputWithAllImages();

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<Exception>()
            .WithMessage("something went wrong.");
        storageServicekMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
        repositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsExceptionInMediaUploadedErrorCase))]
    public async Task ThrowsExceptionInMediaUploadedErrorCase()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidMediaFileInput();
        var expectedBannerName = $"banner.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new Exception("something went wrong."));
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInputWithAllImages();

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<Exception>()
            .WithMessage("something went wrong.");
        storageServicekMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
        repositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadInImageErrorCase))]
    public async Task ThrowsExceptionAndRollbackUploadInImageErrorCase()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var validFileInput = _fixture.GetValidImageFileInput();
        var expectedBannerName = $"banner.{validFileInput.Extension}";
        var expectedThumbName = $"thumb.{validFileInput.Extension}";
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains("-banner")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedBannerName);
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains("-thumb")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(expectedThumbName);
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains("-thumbhalf")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new Exception("something went wrong."));
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInputWithAllImages();

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<Exception>()
            .WithMessage("something went wrong.");
        storageServicekMock.Verify(x => x.DeleteAsync(
                It.Is<string>(x => (x == expectedThumbName) || (x == expectedBannerName)),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2)
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
        repositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
            ), Times.Never
        );
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadInMediaErrorCase))]
    public async Task ThrowsExceptionAndRollbackUploadInMediaErrorCase()
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServicekMock = _fixture.GetStorageService();
        var input = _fixture.GetValidVideoInputWithAllMedias();
        var storageMediaPath = _fixture.GetValidMediaPath();
        var storageTrailerPath = _fixture.GetValidMediaPath();
        var storagePathList = new List<string>() { storageMediaPath, storageTrailerPath };
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains($"media.{input.Media!.Extension}")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(storageMediaPath);
        storageServicekMock.Setup(x => x.UploadAsync(
                It.Is<string>(x => x.Contains($"trailer.{input.Trailer!.Extension}")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(storageTrailerPath);
        unitOfWorkMock.Setup(x => x.CommitAsync(
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new Exception("Something went wrong with the commmit."));
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            storageServicekMock.Object,
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<Exception>()
            .WithMessage("Something went wrong with the commmit.");
        storageServicekMock.Verify(x => x.DeleteAsync(
                It.Is<string>(x => storagePathList.Contains(x)),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2)
        );
        storageServicekMock.Verify(x => x.DeleteAsync(
                It.Is<string>(x => storagePathList.Contains(x)),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2)
        );
        unitOfWorkMock.Verify(x => x.CommitAsync(
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Theory(DisplayName = nameof(CreateVideoThrowsIfInvalidInput))]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsIfInvalidInput(
        CreateVideoInput input,
        string expectedValidationError
    )
    {
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        var expectationAssertion = await action
            .Should()
            .ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("There are validation errors.");
        expectationAssertion
            .Which
            .Errors!.ToList()[0]
            .Message
            .Should().Be(expectedValidationError);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    public async Task CreateVideoWithCategoriesIds()
    {
        var catetoryRepositoryMock = _fixture.GetCategoryRepository();
        var categoriesIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        catetoryRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoriesIds);
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            catetoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(categoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
                && video.Categories.All(categoryId => categoriesIds.Contains(categoryId))
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        catetoryRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        var outputCategoriesItem = output.Categories!.Select(category => category.Id).ToList();
        outputCategoriesItem.Should().BeEquivalentTo(categoriesIds);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoThrowsWhenCategoryIdDoesnExist))]
    public async Task CreateVideoThrowsWhenCategoryIdDoesnExist()
    {
        var categoriesIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        var removedCategoryId = categoriesIds[2];
        var categoryRepositoryMock = _fixture.GetCategoryRepository();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(categoriesIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related category Id (or Ids) not found: {removedCategoryId}.");
        videoRepositoryMock.Verify(x => x.InsertAsync(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        categoryRepositoryMock.VerifyAll();
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    public async Task CreateVideoWithGenresIds()
    {
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var genresIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        genreRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(genresIds);
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(genresIds: genresIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
                && video.Genres.All(genreId => genresIds.Contains(genreId))
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        genreRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Categories.Should().BeNullOrEmpty();
        var outputGenresItem = output.Genres!.Select(genre => genre.Id).ToList();
        outputGenresItem.Should().BeEquivalentTo(genresIds);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenGenreIdInvalid))]
    public async Task ThrowsWhenGenreIdInvalid()
    {
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var genresIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        var removedId = genresIds[2];
        genreRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(genresIds.FindAll(id => id != removedId));
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(genresIds: genresIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related genre Id (or Ids) not found: {removedId}.");
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    public async Task CreateVideoWithCastMembersIds()
    {
        var castMemberRepositoryMock = _fixture.GetCastMemberRepository();
        var castMembersIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(castMembersIds);
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(castMembersIds: castMembersIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.InsertAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id != Guid.Empty
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Rating == input.Rating
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.YearLaunched == input.YearLaunched
                && video.CastMembers.All(castMemberId => castMembersIds.Contains(castMemberId))
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        castMemberRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Categories.Should().BeNullOrEmpty();
        output.Genres.Should().BeNullOrEmpty();
        var outputCastMemberItem = output.CastMembers!.Select(castMember => castMember.Id).ToList();
        outputCastMemberItem.Should().BeEquivalentTo(castMembersIds);
    }

    [Trait("Use Cases", "CreateVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenInvalidCastMemberId))]
    public async Task ThrowsWhenInvalidCastMemberId()
    {
        var castMemberRepositoryMock = _fixture.GetCastMemberRepository();
        var castMembersIds = Enumerable
            .Range(1, 5)
            .Select(_ => Guid.NewGuid())
            .ToList();
        var removedId = castMembersIds[3];
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(castMembersIds.FindAll(x => x != removedId));
        var repositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            Mock.Of<IStorageService>(),
            unitOfWorkMock.Object
        );
        var input = _fixture.GetValidVideoInput(castMembersIds: castMembersIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member Id (or Ids) not found: {removedId}.");
        castMemberRepositoryMock.VerifyAll();
    }
}
