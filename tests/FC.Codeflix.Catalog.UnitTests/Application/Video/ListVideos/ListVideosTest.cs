using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using Entity = FC.Codeflix.Catalog.Domain.Entity.Video;
using FC.Codeflix.Catalog.Domain.Extension;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

[Collection(nameof(ListVideosTestFixture))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;

    public ListVideosTest(ListVideosTestFixture fixture)
        => _fixture = fixture;

    [Trait("Use Cases", "ListVideos - Use Cases")]
    [Fact(DisplayName = nameof(ListVideosOk))]
    public async Task ListVideosOk()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var exampleVideosList = _fixture.VideosList();
        var input = new ListVideosInput(
            page: 1,
            perPage: 10,
            search: "",
            sort: "",
            dir: SearchOrder.ASC
        );
        var outputRepositorySearch = new SearchOutput<Entity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: exampleVideosList,
            total: new Random().Next(50, 200)
        );
        videoRepositoryMock.Setup(x => x.SearchAsync(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page
                    && x.PerPage == input.PerPage
                    && x.Search == input.Search
                    && x.OrderBy == input.Sort
                    && x.SearchOrder == input.Dir
                ),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(outputRepositorySearch);
        var useCase = new UseCase.ListVideos(
            videoRepositoryMock.Object,
            _fixture.GetCategoryRepository().Object,
            _fixture.GetGenreRepository().Object
        );
        var output = await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(output =>
        {
            var exampleVideo = exampleVideosList.Find(x => x.Id == output.Id);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleVideo!.Id);
            output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
            output.Title.Should().Be(exampleVideo.Title);
            output.Description.Should().Be(exampleVideo.Description);
            output.Duration.Should().Be(exampleVideo.Duration);
            output.Rating.Should().Be(exampleVideo.Rating.ToStringRating());
            output.Opened.Should().Be(exampleVideo.Opened);
            output.Published.Should().Be(exampleVideo.Published);
            output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
            output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
            output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
            output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
            var outputCategoriesItem = output.Categories!.Select(category => category.Id).ToList();
            outputCategoriesItem.Should().BeEquivalentTo(exampleVideo.Categories);
            var outputCastMemberItem = output.CastMembers!.Select(castMember => castMember.Id).ToList();
            outputCastMemberItem.Should().BeEquivalentTo(exampleVideo.CastMembers);
            var outputGenresItem = output.Genres!.Select(genre => genre.Id).ToList();
            outputGenresItem.Should().BeEquivalentTo(exampleVideo.Genres);
        });
    }

    [Trait("Use Cases", "ListVideos - Use Cases")]
    [Fact(DisplayName = nameof(ListVideosWithRelations))]
    public async Task ListVideosWithRelations()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var categoryRepositoryMock = _fixture.GetCategoryRepository();
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var (Videos,
            Categories,
            Genres
        ) = _fixture.VideosListWithRelations();
        var examplesCategoriesIds = Categories.Select(x => x.Id).ToList();
        var examplesGenresIds = Genres.Select(x => x.Id).ToList();
        var input = new ListVideosInput(
            page: 1,
            perPage: 10,
            search: "",
            sort: "",
            dir: SearchOrder.ASC
        );
        var outputRepositorySearch = new SearchOutput<Entity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: Videos,
            total: new Random().Next(50, 200)
        );
        categoryRepositoryMock.Setup(x => x.GetListByIdsAsync(
                It.Is<List<Guid>>(list =>
                    list.All(examplesCategoriesIds.Contains)
                    && list.Count == examplesCategoriesIds.Count
                ),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(Categories);
        genreRepositoryMock.Setup(x => x.GetListByIdsAsync(
                It.Is<List<Guid>>(list =>
                    list.All(examplesGenresIds.Contains)
                    && list.Count == examplesGenresIds.Count
                ),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(Genres);
        videoRepositoryMock.Setup(x => x.SearchAsync(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page
                    && x.PerPage == input.PerPage
                    && x.Search == input.Search
                    && x.OrderBy == input.Sort
                    && x.SearchOrder == input.Dir
                ),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(outputRepositorySearch);
        var useCase = new UseCase.ListVideos(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object
        );
        var output = await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        categoryRepositoryMock.VerifyAll();
        genreRepositoryMock.VerifyAll();
        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(output =>
        {
            var exampleVideo = Videos.Find(x => x.Id == output.Id);
            output.Should().NotBeNull();
            output.Id.Should().Be(exampleVideo!.Id);
            output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
            output.Title.Should().Be(exampleVideo.Title);
            output.Description.Should().Be(exampleVideo.Description);
            output.Duration.Should().Be(exampleVideo.Duration);
            output.Rating.Should().Be(exampleVideo.Rating.ToStringRating());
            output.Opened.Should().Be(exampleVideo.Opened);
            output.Published.Should().Be(exampleVideo.Published);
            output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
            output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
            output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
            output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
            output.Categories!.ToList().ForEach(relation =>
            {
                var exampleCategory = Categories.Find(x => x.Id == relation.Id);
                exampleCategory.Should().NotBeNull();
                relation.Name.Should().Be(exampleCategory?.Name);
            });
            output.Genres!.ToList().ForEach(relation =>
            {
                var exampleGenres = Genres.Find(x => x.Id == relation.Id);
                exampleGenres.Should().NotBeNull();
                relation.Name.Should().Be(exampleGenres?.Name);
            });
            var outputCastMemberItem = output.CastMembers!.Select(castMember => castMember.Id).ToList();
            outputCastMemberItem.Should().BeEquivalentTo(exampleVideo.CastMembers);
        });
    }

    [Trait("Use Cases", "ListVideos - Use Cases")]
    [Fact(DisplayName = nameof(ListVideosWithoutRelations))]
    public async Task ListVideosWithoutRelations()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var categoryRepositoryMock = _fixture.GetCategoryRepository();
        var genreRepositoryMock = _fixture.GetGenreRepository();
        var exampleVideos = _fixture.VideosListWithoutRelations();
        var input = new ListVideosInput(
            page: 1,
            perPage: 10,
            search: "",
            sort: "",
            dir: SearchOrder.ASC
        );
        var outputRepositorySearch = new SearchOutput<Entity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: exampleVideos,
            total: new Random().Next(50, 200)
        );
        videoRepositoryMock.Setup(x => x.SearchAsync(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page
                    && x.PerPage == input.PerPage
                    && x.Search == input.Search
                    && x.OrderBy == input.Sort
                    && x.SearchOrder == input.Dir
                ),
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(outputRepositorySearch);
        var useCase = new UseCase.ListVideos(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object
        );
        var output = await useCase.Handle(input, CancellationToken.None);

        videoRepositoryMock.VerifyAll();
        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ToList().ForEach(output =>
        {
            var exampleVideo = exampleVideos.Find(x => x.Id == output.Id);
            output.Should().NotBeNull();
            output.Id.Should().Be(exampleVideo!.Id);
            output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
            output.Title.Should().Be(exampleVideo.Title);
            output.Description.Should().Be(exampleVideo.Description);
            output.Duration.Should().Be(exampleVideo.Duration);
            output.Rating.Should().Be(exampleVideo.Rating.ToStringRating());
            output.Opened.Should().Be(exampleVideo.Opened);
            output.Published.Should().Be(exampleVideo.Published);
            output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
            output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
            output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
            output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
            output.Categories.Should().BeNullOrEmpty();
            output.Genres.Should().BeNullOrEmpty();
            output.CastMembers.Should().BeNullOrEmpty();
        });
        categoryRepositoryMock.Verify(x => x.GetListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
        genreRepositoryMock.Verify(x => x.GetListByIdsAsync(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
    }

    [Trait("Use Cases", "ListVideos - Use Cases")]
    [Fact(DisplayName = nameof(ListReturnEmptyWhenThereIsNoVideo))]
    public async Task ListReturnEmptyWhenThereIsNoVideo()
    {
        var videoRepositoryMock = _fixture.GetRepository();
        var input = new ListVideosInput(
            page: 1,
            perPage: 10,
            search: "",
            sort: "",
            dir: SearchOrder.ASC
        );
        var outputRepositorySearch = new SearchOutput<Entity>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Entity>(),
            total: 0
        );
        videoRepositoryMock.Setup(x => x.SearchAsync(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page
                    && x.PerPage == input.PerPage
                    && x.Search == input.Search
                    && x.OrderBy == input.Sort
                    && x.SearchOrder == input.Dir
                ),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(outputRepositorySearch);
        var useCase = new UseCase.ListVideos(
            videoRepositoryMock.Object,
            _fixture.GetCategoryRepository().Object,
            _fixture.GetGenreRepository().Object
        );
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }
}
