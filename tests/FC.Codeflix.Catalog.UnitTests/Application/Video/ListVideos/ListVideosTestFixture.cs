using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture> { }

public class ListVideosTestFixture : VideoUseCaseBaseFixture
{
    public List<DomainEntity.Video> VideosList() 
        => Enumerable
            .Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideoAllProperties())
            .ToList();

    public (
        List<DomainEntity.Video> Videos, 
        List<DomainEntity.Category> Categories,
        List<DomainEntity.Genre> Genres
    ) VideosListWithRelations()
    {
        var itemsToBeCreated = Random.Shared.Next(2, 10);
        var categories = new List<DomainEntity.Category>();
        var genres = new List<DomainEntity.Genre>();

        var videos = Enumerable
            .Range(1, itemsToBeCreated)
            .Select(_ => GetValidVideoAllProperties())
            .ToList();

        videos.ForEach(video =>
        {
            video.RemoveAllCategories();
            var qtdCategories = Random.Shared.Next(1, 5);
            for (int i = 0; i < qtdCategories; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres = Random.Shared.Next(1, 5);
            for (int i = 0; i < qtdCategories; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }
        });

        return (videos, categories, genres);
    }

    private DomainEntity.Category GetExampleCategory() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    private string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[..255];

        return categoryName;
    }

    private string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];

        return categoryDescription;
    }

    private DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {

        var genre = new DomainEntity.Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    private string GetValidGenreName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        return categoryName;
    }
}
