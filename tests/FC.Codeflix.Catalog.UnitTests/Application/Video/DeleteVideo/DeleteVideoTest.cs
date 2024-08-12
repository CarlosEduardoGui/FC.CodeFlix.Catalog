using Moq;
using Xunit;
using VideoEntity = FC.Codeflix.Catalog.Domain.Entity.Video;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.DeleteVideo;

[Collection(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTest
{
    private readonly DeleteVideoTestFixture _fixture;

    public DeleteVideoTest(DeleteVideoTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "DeleteVideo - Use Cases")]
    [Fact(DisplayName = nameof(DeleteVideoOk))]
    public async Task DeleteVideoOk()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var useCase = new UseCase.DeleteVideo(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object
        );
        var videoExample = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(videoExample.Id);
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoExample);

        await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        videoRepositoryMock.Verify(x => x.DeleteAsync(
            It.Is<VideoEntity>(x => x.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "DeleteVideo - Use Cases")]
    [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndClearStorage))]
    public async Task DeleteVideoWithAllMediasAndClearStorage()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var useCase = new UseCase.DeleteVideo(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object);
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        var filesPath = new List<string>()
        {
            videoExample.Media!.FilePath,
            videoExample.Trailer!.FilePath
        };
        var input = _fixture.GetValidInput(videoExample.Id);
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoExample);

        await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        videoRepositoryMock.Verify(x => x.DeleteAsync(
            It.Is<VideoEntity>(x => x.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.Is<string>(filePath => filesPath.Contains(filePath)),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "DeleteVideo - Use Cases")]
    [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer))]
    public async Task DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var useCase = new UseCase.DeleteVideo(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object);
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(videoExample.Id);
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoExample);

        await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        videoRepositoryMock.Verify(x => x.DeleteAsync(
            It.Is<VideoEntity>(x => x.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "DeleteVideo - Use Cases")]
    [Fact(DisplayName = nameof(DeleteVideoWithOnlyVideoAndClearStorageOnlyForVideo))]
    public async Task DeleteVideoWithOnlyVideoAndClearStorageOnlyForVideo()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var useCase = new UseCase.DeleteVideo(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object);
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(videoExample.Id);
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoExample);

        await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        videoRepositoryMock.Verify(x => x.DeleteAsync(
            It.Is<VideoEntity>(x => x.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.Is<string>(filePath => filePath == videoExample.Media!.FilePath),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "DeleteVideo - Use Cases")]
    [Fact(DisplayName = nameof(DeleteVideoWithoutVideoAndDontClearStorage))]
    public async Task DeleteVideoWithoutVideoAndDontClearStorage()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var useCase = new UseCase.DeleteVideo(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object);
        var videoExample = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(videoExample.Id);
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(videoExample);

        await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        videoRepositoryMock.Verify(x => x.DeleteAsync(
            It.Is<VideoEntity>(x => x.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "DeleteVideo - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    public async Task ThrowsWhenVideoNotFound()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var useCase = new UseCase.DeleteVideo(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object);
        var videoExample = Guid.NewGuid();
        var input = _fixture.GetValidInput(videoExample);
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == input.Id),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException("Video not found."));

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Video not found.");
        videoRepositoryMock.VerifyAll();
        videoRepositoryMock.Verify(x => x.DeleteAsync(
            It.IsAny<VideoEntity>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
