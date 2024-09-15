using FC.Codeflix.Catalog.Domain.Extension;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;
public record VideoModelOutput(
    Guid Id,
    string Title,
    string Description,
    bool Published,
    int Duration,
    string Rating,
    int YearLaunched,
    bool Opened,
    DateTime CreatedAt,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate>? Categories = null,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate>? Genres = null,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate>? CastMembers = null,
    string? ThumbFileUrl = null,
    string? BannerFileUrl = null,
    string? ThumbHalfFileUrl = null,
    string? VideoFileUrl = null,
    string? TrailerFileUrl = null
)
{
    public static VideoModelOutput FromVideo(
        DomainEntity.Video video,
        IReadOnlyList<DomainEntity.Category>? categories = null,
        IReadOnlyList<DomainEntity.Genre>? genres = null
    ) => new(
        video.Id,
        video.Title,
        video.Description,
        video.Published,
        video.Duration,
        video.Rating.ToStringRating(),
        video.YearLaunched,
        video.Opened,
        video.CreatedAt,
        video.Categories.Select(id => new VideoModelOutputRelatedAggregate(id, categories?.FirstOrDefault(x => x.Id == id)?.Name)).ToList(),
        video.Genres.Select(id => new VideoModelOutputRelatedAggregate(id, genres?.FirstOrDefault(x => x.Id == id)?.Name)).ToList(),
        video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
        video.Thumb?.Path,
        video.Banner?.Path,
        video.ThumbHalf?.Path,
        video.Media?.FilePath,
        video.Trailer?.FilePath
    );
}

public record VideoModelOutputRelatedAggregate(
    Guid Id,
    string? Name = null
);