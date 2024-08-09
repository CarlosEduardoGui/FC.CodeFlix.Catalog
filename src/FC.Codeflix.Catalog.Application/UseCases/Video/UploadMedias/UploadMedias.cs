using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using VideoEntity = FC.Codeflix.Catalog.Domain.Entity.Video;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
public class UploadMedias : IUploadVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadMedias(
        IVideoRepository videoRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork
    )
    {
        _videoRepository = videoRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UploadMediasInput request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetByIdAsync(request.VideoId, cancellationToken);

        try
        {
            await UploadVideo(request, video, cancellationToken);
            await UploadTrailer(request, video, cancellationToken);
            await _videoRepository.UpdateAsync(video, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(request, video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(UploadMediasInput request, VideoEntity video, CancellationToken cancellationToken)
    {
        if (request.VideoFile is not null && video.Media is not null)
            await _storageService.DeleteAsync(video.Media.FilePath, cancellationToken);

        if (request.TrailerFile is not null && video.Trailer is not null)
            await _storageService.DeleteAsync(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadVideo(UploadMediasInput request, VideoEntity video, CancellationToken cancellationToken)
    {
        if (request.VideoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), request.VideoFile.Extension);

            var uploadedFilePath = await _storageService.UploadAsync(
                fileName,
                request.VideoFile.FileStream,
                cancellationToken
            );

            video.UpdateMedia(uploadedFilePath);
        }
    }

    private async Task UploadTrailer(UploadMediasInput request, VideoEntity video, CancellationToken cancellationToken)
    {
        if (request.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), request.TrailerFile.Extension);

            var uploadedFilePath = await _storageService.UploadAsync(
                fileName,
                request.TrailerFile.FileStream,
                cancellationToken
            );

            video.UpdateTrailer(uploadedFilePath);
        }
    }
}
