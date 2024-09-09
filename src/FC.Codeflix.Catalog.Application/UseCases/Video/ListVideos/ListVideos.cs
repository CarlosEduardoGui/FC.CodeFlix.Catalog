using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;

    public ListVideos(
        IVideoRepository videoRepository,
        ICategoryRepository categoryRepository,
        IGenreRepository genreRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
    {

        var searchResult = await _videoRepository.SearchAsync(request.ToSearchInput(),
            cancellationToken);

        IReadOnlyList<DomainEntity.Category>? categoriesList = await CategoriesValidation(searchResult, cancellationToken);

        IReadOnlyList<DomainEntity.Genre>? genresList = await GenresValidation(searchResult, cancellationToken);

        return new ListVideosOutput(
            searchResult.CurrentPage,
            searchResult.PerPage,
            searchResult.Items.Select(item => VideoModelOutput.FromVideo(item, categoriesList, genresList)).ToList(),
            searchResult.Total
        );
    }

    private async Task<IReadOnlyList<DomainEntity.Genre>?> GenresValidation(SearchOutput<DomainEntity.Video> searchResult, CancellationToken cancellationToken)
    {
        IReadOnlyList<DomainEntity.Genre>? genresList = null;
        var relatedGenresIds = searchResult.Items
                    .SelectMany(videos => videos.Genres)
                    .Distinct()
                    .ToList();

        if (relatedGenresIds is not null && relatedGenresIds.Any())
            genresList = await _genreRepository.GetListByIdsAsync(relatedGenresIds, cancellationToken);
        return genresList;
    }

    private async Task<IReadOnlyList<DomainEntity.Category>?> CategoriesValidation(SearchOutput<DomainEntity.Video> searchResult, CancellationToken cancellationToken)
    {
        IReadOnlyList<DomainEntity.Category>? categoriesList = null;
        var relatedCategoriesIds = searchResult.Items
            .SelectMany(videos => videos.Categories)
            .Distinct()
            .ToList();

        if (relatedCategoriesIds is not null && relatedCategoriesIds.Any())
            categoriesList = await _categoryRepository.GetListByIdsAsync(relatedCategoriesIds, cancellationToken);
        return categoriesList;
    }
}
