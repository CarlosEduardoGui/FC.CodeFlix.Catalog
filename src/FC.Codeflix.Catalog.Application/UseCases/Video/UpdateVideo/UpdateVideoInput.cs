using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
public record UpdateVideoInput(
    Guid Id,
    string Title,
    string Description,
    bool Published,
    int Duration,
    Rating Rating,
    int YearLaunched,
    bool Opened,
    List<Guid>? GenresIds = null
) : IRequest<VideoModelOutput>;
