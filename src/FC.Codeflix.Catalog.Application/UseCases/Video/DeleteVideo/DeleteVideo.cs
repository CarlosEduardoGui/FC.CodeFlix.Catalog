using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
public class DeleteVideo : IDeleteVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVideo(
        IVideoRepository videoRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork
    )
    {
        _videoRepository = videoRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetByIdAsync(request.Id, cancellationToken);

        await _videoRepository.DeleteAsync(video, cancellationToken);

        if(video.Trailer is not null)
            await _storageService.DeleteAsync(video.Trailer.FilePath, cancellationToken);

        if (video.Media is not null)
            await _storageService.DeleteAsync(video.Media.FilePath, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
