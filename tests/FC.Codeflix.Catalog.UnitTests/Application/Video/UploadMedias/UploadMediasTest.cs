using FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedia;
using FC.Codeflix.Catalog.Application.Common;
using Xunit;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[Collection(nameof(UploadMediasTestFixture))]
public class UploadMediasTest
{
    private readonly UploadMediasTestFixture _fixture;

    public UploadMediasTest(UploadMediasTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "UploadMedias - Use Cases")]
    [Fact(DisplayName = nameof(UploadMediaOk))]
    public async Task UploadMediaOk()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var video = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(video.Id);
        var fileNames = new List<string>()
        {
            StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile!.Extension),
            StorageFileName.Create(video.Id, nameof(video.Trailer), input.VideoFile!.Extension)
        };
        var useCase = new UseCase.UploadMedias(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object
        );
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(video);
        storageServiceMock.Setup(x => x.UploadAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(Guid.NewGuid().ToString());

        await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        storageServiceMock.Verify(x => x.UploadAsync(
            It.Is<string>(x => fileNames.Contains(x)),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "UploadMedias - Use Cases")]
    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    public async Task ThrowsWhenVideoNotFound()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var video = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(video.Id);
        var useCase = new UseCase.UploadMedias(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object
        );
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new NotFoundException("Video not found."));

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Video not found.");
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Trait("Use Cases", "UploadMedias - Use Cases")]
    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    public async Task ClearStorageInUploadErrorCase()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var video = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(video.Id);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile!.Extension);
        var trailerFileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.VideoFile!.Extension);
        var fileNames = new List<string>() { videoFileName, trailerFileName };
        var videoStoragePath = $"storage/{videoFileName}";
        var trailerStoragePath = $"storage/{trailerFileName}";
        var useCase = new UseCase.UploadMedias(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object
        );
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(video);
        storageServiceMock.Setup(x => x.UploadAsync(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(videoStoragePath);
        storageServiceMock.Setup(x => x.UploadAsync(
            It.Is<string>(x => x == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new Exception("Something went wrong with the upload."));

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("Something went wrong with the upload.");
        videoRepositoryMock.VerifyAll();
        storageServiceMock.Verify(x => x.UploadAsync(
            It.Is<string>(x => fileNames.Contains(x)),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.Is<string>(fileName => fileName == videoStoragePath),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Trait("Use Cases", "UploadMedias - Use Cases")]
    [Fact(DisplayName = nameof(ClearStorageInCommitErrorCase))]
    public async Task ClearStorageInCommitErrorCase()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var video = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(video.Id);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile!.Extension);
        var trailerFileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.VideoFile!.Extension);
        var fileNames = new List<string>() { videoFileName, trailerFileName };
        var videoStoragePath = $"storage/{videoFileName}";
        var trailerStoragePath = $"storage/{trailerFileName}";
        var filePathNames = new List<string>() { videoStoragePath, trailerStoragePath };
        var useCase = new UseCase.UploadMedias(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object
        );
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(video);
        storageServiceMock.Setup(x => x.UploadAsync(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(videoStoragePath);
        storageServiceMock.Setup(x => x.UploadAsync(
            It.Is<string>(x => x == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(trailerStoragePath);
        unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong with the upload."));

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("Something went wrong with the upload.");
        videoRepositoryMock.VerifyAll();
        storageServiceMock.Verify(x => x.UploadAsync(
            It.Is<string>(x => fileNames.Contains(x)),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.Is<string>(fileName => filePathNames.Contains(fileName)),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Trait("Use Cases", "UploadMedias - Use Cases")]
    [Fact(DisplayName = nameof(ClearOnlyOneFileInStorageInCommitErrorCaseIfProvideOnlyOneFile))]
    public async Task ClearOnlyOneFileInStorageInCommitErrorCaseIfProvideOnlyOneFile()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var storageServiceMock = _fixture.GetStorageService();
        var video = _fixture.GetValidVideo();
        video.UpdateTrailer(_fixture.GetValidMediaPath());
        video.UpdateMedia(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(video.Id, withTrailerFile: false);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile!.Extension);
        var videoStoragePath = $"storage/{videoFileName}";
        var useCase = new UseCase.UploadMedias(
            videoRepositoryMock.Object,
            storageServiceMock.Object,
            unitOfWorkMock.Object
        );
        videoRepositoryMock.Setup(x => x.GetByIdAsync(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(video);
        storageServiceMock.Setup(x => x.UploadAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(Guid.NewGuid().ToString());
        storageServiceMock.Setup(x => x.UploadAsync(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(videoStoragePath);
        unitOfWorkMock.Setup(x => x.CommitAsync(
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong with the upload."));

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("Something went wrong with the upload.");
        videoRepositoryMock.VerifyAll();
        storageServiceMock.Verify(x => x.UploadAsync(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        storageServiceMock.Verify(x => x.DeleteAsync(
            It.Is<string>(fileName => fileName == videoStoragePath),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
