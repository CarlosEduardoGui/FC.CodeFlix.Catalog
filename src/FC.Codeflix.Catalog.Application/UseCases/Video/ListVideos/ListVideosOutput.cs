using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
public class ListVideosOutput : PaginatedListOuput<VideoModelOutput>
{
    public ListVideosOutput(
        int currentPage,
        int perPage,
        IReadOnlyList<VideoModelOutput> items,
        int total) : base(currentPage, perPage, items, total)
    {
    }
}
