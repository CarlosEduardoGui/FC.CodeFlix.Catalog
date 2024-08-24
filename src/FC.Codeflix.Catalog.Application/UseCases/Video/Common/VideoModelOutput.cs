using FC.Codeflix.Catalog.Domain.Enum;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;
public record VideoModelOutput(
    Guid Id,
    string Title,
    string Description,
    bool Published,
    int Duration,
    Rating Rating,
    int YearLaunched,
    bool Opened,
    DateTime CreatedAt,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null,
    IReadOnlyCollection<Guid>? CastMembersIds = null,
    string? Thumb = null,
    string? Banner = null,
    string? ThumbHalf = null,
    string? Media = null,
    string? Trailer = null
)
{
    public static VideoModelOutput FromVideo(DomainEntity.Video video)
        => new(
            video.Id,
            video.Title,
            video.Description,
            video.Published,
            video.Duration,
            video.Rating,
            video.YearLaunched,
            video.Opened,
            video.CreatedAt,
            video.Categories,
            video.Genres,
            video.CastMembers,
            video.Thumb?.Path,
            video.Banner?.Path,
            video.ThumbHalf?.Path,
            video.Media?.FilePath,
            video.Trailer?.FilePath
        );
}
