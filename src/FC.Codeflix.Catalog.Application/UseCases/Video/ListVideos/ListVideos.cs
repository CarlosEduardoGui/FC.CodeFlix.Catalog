using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;

    public ListVideos(IVideoRepository videoRepository) 
        => _videoRepository = videoRepository;

    public async Task<ListVideosOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
    {

        var searchResult = await _videoRepository.SearchAsync(request.ToSearchInput(),
            cancellationToken);

        return new ListVideosOutput(
            searchResult.CurrentPage, 
            searchResult.PerPage, 
            searchResult.Items.Select(VideoModelOutput.FromVideo).ToList(), 
            searchResult.Total
        );
    }
}
