using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VideoModelOutput> Handle(UpdateVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetByIdAsync(request.Id, cancellationToken);

        video.Update(
            request.Title,
            request.Description,
            request.YearLaunched,
            request.Opened,
            request.Published,
            request.Duration,
            request.Rating
        );

        await _videoRepository.UpdateAsync(video, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return VideoModelOutput.FromVideo(video);
    }
}
