using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using Entity = FC.Codeflix.Catalog.Domain.Entity.Video;

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
        var useCase = new UseCase.ListVideos(videoRepositoryMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);

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
            output.Rating.Should().Be(exampleVideo.Rating);
            output.Opened.Should().Be(exampleVideo.Opened);
            output.Published.Should().Be(exampleVideo.Published);
            output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            output.Thumb.Should().Be(exampleVideo.Thumb!.Path);
            output.ThumbHalf.Should().Be(exampleVideo.ThumbHalf!.Path);
            output.Media.Should().Be(exampleVideo.Media!.FilePath);
            output.Trailer.Should().Be(exampleVideo.Trailer!.FilePath);
            output.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
            output.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
            output.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
        });
    }
}
