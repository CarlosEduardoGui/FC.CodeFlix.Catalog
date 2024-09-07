using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;
using FC.Codeflix.Catalog.Domain.Extension;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.GetVideo;

[Collection(nameof(GetVideoTestFixture))]
public class GetVideoTest
{
    private readonly GetVideoTestFixture _fixture;

    public GetVideoTest(GetVideoTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "GetVideo - Use Cases")]
    [Fact(DisplayName = nameof(GetVideoOk))]
    public async Task GetVideoOk()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var exampleVideo = _fixture.GetValidVideo();
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.GetVideo(videoRepositoryMock.Object);
        var input = new GetVideoInput(exampleVideo.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringRating());
        output.Opened.Should().Be(exampleVideo  .Opened);
        output.Published.Should().Be(exampleVideo.Published);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        videoRepositoryMock.VerifyAll();
    }

    [Trait("Use Cases", "GetVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    public async Task ThrowsWhenNotFound()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new NotFoundException("Video not found."));
        var useCase = new UseCase.GetVideo(videoRepositoryMock.Object);
        var input = new GetVideoInput(Guid.NewGuid());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Video not found.");
    }

    [Trait("Use Cases", "GetVideo - Use Cases")]
    [Fact(DisplayName = nameof(GetVideoWithAllProperties))]
    public async Task GetVideoWithAllProperties()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var exampleVideo = _fixture.GetValidVideoAllProperties();
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.GetVideo(videoRepositoryMock.Object);
        var input = new GetVideoInput(exampleVideo.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringRating());
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Published.Should().Be(exampleVideo.Published);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
        output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
        var outputCategoriesItem = output.Categories!.Select(category => category.Id).ToList();
        outputCategoriesItem.Should().BeEquivalentTo(exampleVideo.Categories);
        var outputCastMemberItem = output.CastMembers!.Select(castMember => castMember.Id).ToList();
        outputCastMemberItem.Should().BeEquivalentTo(exampleVideo.CastMembers);
        var outputGenresItem = output.Genres!.Select(genre => genre.Id).ToList();
        outputGenresItem.Should().BeEquivalentTo(exampleVideo.Genres);
        videoRepositoryMock.VerifyAll();
    }
}
