using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVideo(
        IVideoRepository videoRepository, 
        IGenreRepository genreRepository,
        IUnitOfWork unitOfWork)
    {
        _genreRepository = genreRepository;
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

        await ValidateAndAddRelations(request, video, cancellationToken);

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException(
                "There are validation errors.",
                validationHandler.Errors
            );

        await _videoRepository.UpdateAsync(video, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return VideoModelOutput.FromVideo(video);
    }

    private async Task ValidateAndAddRelations(UpdateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
    {
        if ((request.GenresIds?.Count ?? 0) > 0)
        {
            await ValidateGenresIds(request, cancellationToken);

            request.GenresIds!.ToList().ForEach(video.AddGenre);
        }
    }

    private async Task ValidateGenresIds(UpdateVideoInput request, CancellationToken cancellationToken)
    {
        var persistenceIds = await _genreRepository.GetIdsListByIdsAsync(
                        request.GenresIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < request.GenresIds!.Count)
        {
            var notFoundIds = request.GenresIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }
}
