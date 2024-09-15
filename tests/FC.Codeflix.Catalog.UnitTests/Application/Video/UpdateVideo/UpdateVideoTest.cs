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

    [Fact]
    [Trait("Use Cases", "UpdateVideo - Use Cases")]
    public async void UpdateVideoOk()
    {
        var videoRepository = _fixture.GetRepository();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        videoRepository.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(id => id == exampleVideo.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.UpdateVideo(videoRepository.Object, unitOfWork.Object);

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
}
