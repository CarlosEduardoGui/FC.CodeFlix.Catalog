using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;
public class GetVideo : IGetVideo
{
    private readonly IVideoRepository _videoRepository;

    public GetVideo(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<VideoModelOutput> Handle(GetVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetByIdAsync(request.Id, cancellationToken);

        return VideoModelOutput.FromVideo(video);
    }
}
