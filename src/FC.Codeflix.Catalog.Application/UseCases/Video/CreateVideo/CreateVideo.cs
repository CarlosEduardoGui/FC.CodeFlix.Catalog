using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _uow;

    public CreateVideo(
        IVideoRepository repository,
        ICategoryRepository categoryRepository,
        IGenreRepository genreRepository,
        ICastMemberRepository castMemberRepository,
        IStorageService storageService,
        IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _castMemberRepository = castMemberRepository;
        _storageService = storageService;
    }

    public async Task<VideoModelOutput> Handle(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var video = new DomainEntity.Video(
            request.Title,
            request.Description,
            request.YearLaunched,
            request.Opened,
            request.Published,
            request.Duration,
            request.Rating
        );

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException(
                "There are validation errors.",
                validationHandler.Errors
            );

        await ValidateAndAddRelations(request, video, cancellationToken);
        try
        {
            await UploadImagesMedia(request, video, cancellationToken);

            await UploadVideosMedia(request, video, cancellationToken);

            await _repository.InsertAsync(video, cancellationToken);

            await _uow.CommitAsync(cancellationToken);

            return VideoModelOutput.FromVideo(video);
        }
        catch (Exception)
        {
            await ClearStorage(video, cancellationToken);

            throw;
        }
    }

    private async Task ClearStorage(DomainEntity.Video video, CancellationToken cancellationToken)
    {
        if (video.Thumb is not null)
            await _storageService.DeleteAsync(video.Thumb.Path, cancellationToken);

        if (video.ThumbHalf is not null)
            await _storageService.DeleteAsync(video.ThumbHalf.Path, cancellationToken);

        if (video.Banner is not null)
            await _storageService.DeleteAsync(video.Banner.Path, cancellationToken);

        if (video.Media is not null)
            await _storageService.DeleteAsync(video.Media.FilePath, cancellationToken);

        if (video.Trailer is not null)
            await _storageService.DeleteAsync(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadImagesMedia(CreateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
    {
        if (request.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), request.Thumb.Extension);

            var urlThumb = await _storageService.UploadAsync(fileName, request.Thumb.FileStream, cancellationToken);

            video.UpdateThumb(urlThumb);
        }

        if (request.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), request.Banner.Extension);

            var urlBanner = await _storageService.UploadAsync(fileName, request.Banner.FileStream, cancellationToken);

            video.UpdateBanner(urlBanner);
        }

        if (request.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), request.ThumbHalf.Extension);

            var urlBanner = await _storageService.UploadAsync(fileName, request.ThumbHalf.FileStream, cancellationToken);

            video.UpdateThumbHalf(urlBanner);
        }
    }

    private async Task ValidateAndAddRelations(CreateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
    {
        if ((request.CategoriesIds?.Count ?? 0) > 0)
        {
            await ValidateCategoriesIds(request, cancellationToken);

            request.CategoriesIds!
                .ToList()
                .ForEach(video.AddCategory);
        }

        if ((request.GenresIds?.Count ?? 0) > 0)
        {
            await ValidateGenresIds(request, cancellationToken);

            request.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        if ((request.CastMembersIds?.Count ?? 0) > 0)
        {
            await ValidatedCastMembersIds(request, cancellationToken);

            request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidatedCastMembersIds(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var persistenceIds = await _castMemberRepository.GetIdsListByIdsAsync(
                        request.CastMembersIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < request.CastMembersIds!.Count)
        {
            var notFoundIds = request.CastMembersIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related cast member id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateGenresIds(CreateVideoInput request, CancellationToken cancellationToken)
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

    private async Task ValidateCategoriesIds(CreateVideoInput request, CancellationToken cancellationToken)
    {
        var persistenceIds = await _categoryRepository.GetIdsListByIdsAsync(
                        request.CategoriesIds!.ToList(),
                        cancellationToken
                    );

        if (persistenceIds.Count < request.CategoriesIds!.Count)
        {
            var notFoundIds = request.CategoriesIds
                .ToList()
                .FindAll(categoryId => persistenceIds.Contains(categoryId) is false);

            var stringNotFoundIds = string.Join(',', notFoundIds);

            throw new RelatedAggregateException($"Related category Id (or Ids) not found: {stringNotFoundIds}.");
        }
    }

    private async Task UploadVideosMedia(CreateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
    {
        if (request.Media is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), request.Media.Extension);

            var urlMedia = await _storageService.UploadAsync(fileName, request.Media.FileStream, cancellationToken);

            video.UpdateMedia(urlMedia);
        }

        if (request.Trailer is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), request.Trailer.Extension);

            var urlMedia = await _storageService.UploadAsync(fileName, request.Trailer.FileStream, cancellationToken);

            video.UpdateTrailer(urlMedia);
        }
    }
}
