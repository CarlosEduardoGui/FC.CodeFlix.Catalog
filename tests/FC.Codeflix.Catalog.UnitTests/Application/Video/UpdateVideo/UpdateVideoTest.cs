using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Extension;
using FluentAssertions;
using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateVideoOk))]
    [Trait("Use Cases", "UpdateVideo - Use Cases")]
    public async void UpdateVideoOk()
    {
        var videoRepository = _fixture.GetRepository();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        videoRepository.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(id => id == exampleVideo.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.UpdateVideo(videoRepository.Object, genreRepositoryMock.Object, unitOfWork.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        videoRepository.VerifyAll();
        videoRepository.Verify(repository => repository.UpdateAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.YearLaunched == input.YearLaunched
            ),
            It.IsAny<CancellationToken>()),
        Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()),
        Times.Once);
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

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenVideoNotFound))]
    [Trait("Use Cases", "UpdateVideo - Use Cases")]
    public async void UpdateVideoThrowsWhenVideoNotFound()
    {
        var videoRepository = _fixture.GetRepository();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var input = _fixture.CreateValidInput(Guid.NewGuid());
        videoRepository.Setup(x => x.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException("Video not found."));
        var useCase = new UseCase.UpdateVideo(videoRepository.Object, genreRepositoryMock.Object, unitOfWork.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Video not found.");
        videoRepository.Verify(repository => repository.UpdateAsync(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
        unitOfWork.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()),
        Times.Never);
    }

    [Theory(DisplayName = nameof(UpdateVideoThrowsWhenReceiveInvalidInput))]
    [Trait("Use Cases", "UpdateVideo - Use Cases")]
    [ClassData(typeof(UpdateVideoTestDataGenerator))]
    public async void UpdateVideoThrowsWhenReceiveInvalidInput(UpdateVideoInput invalidInput, string expectedExceptionMessage)
    {
        var videoRepository = _fixture.GetRepository();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var exampleVideo = _fixture.GetValidVideo();
        videoRepository.Setup(x => x.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.UpdateVideo(videoRepository.Object, genreRepositoryMock.Object, unitOfWork.Object);

        var action = async () => await useCase.Handle(invalidInput, CancellationToken.None);

        var expectationAssertion = await action.Should()
            .ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("There are validation errors.");
        expectationAssertion
            .Which
            .Errors!.ToList()[0]
            .Message
            .Should().Be(expectedExceptionMessage);
        videoRepository.Verify(x => x.UpdateAsync(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
        unitOfWork.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()),
        Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithGenreIds))]
    [Trait("Use Cases", "UpdateVideo - Use Cases")]
    public async void UpdateVideoWithGenreIds()
    {
        var videoRepository = _fixture.GetRepository();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var exampleVideo = _fixture.GetValidVideo();
        var exampleGenreIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, exampleGenreIds);
        videoRepository.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(id => id == exampleVideo.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        genreRepositoryMock.Setup(x => x.GetIdsListByIdsAsync(
            It.Is<List<Guid>>(idsList =>
                idsList.Count == exampleGenreIds.Count
                && idsList.All(id => exampleGenreIds.Contains(id))
            ),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleGenreIds);
        var useCase = new UseCase.UpdateVideo(videoRepository.Object, genreRepositoryMock.Object, unitOfWork.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        videoRepository.VerifyAll();
        genreRepositoryMock.VerifyAll();
        videoRepository.Verify(repository => repository.UpdateAsync(
            It.Is<DomainEntity.Video>(video =>
                video.Id == exampleVideo.Id
                && video.Title == input.Title
                && video.Description == input.Description
                && video.Duration == input.Duration
                && video.Opened == input.Opened
                && video.Published == input.Published
                && video.Rating == input.Rating
                && video.YearLaunched == input.YearLaunched
                && video.Genres.All(id => exampleGenreIds.Contains(id)
                    && video.Genres.Count == exampleGenreIds.Count)
            ),
            It.IsAny<CancellationToken>()),
        Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()),
        Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringRating());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Genres.Should().NotBeNullOrEmpty();
        output.Genres!.Select(genre => genre.Id)
            .ToList()
            .Should()
            .BeEquivalentTo(exampleGenreIds);
    }
}
