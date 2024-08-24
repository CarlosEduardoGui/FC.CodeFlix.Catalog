using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.ApiModels;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMembers;

[Collection(nameof(CastMemberBaseFixture))]
public class ListCastMembersTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public ListCastMembersTest(CastMemberBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "CastMember/ListCastMember - Endpoints")]
    [Fact(DisplayName = nameof(List))]
    public async Task List()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(10);
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/cast_members");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(exampleCastMembersList.Count);
        output.Meta!.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(exampleCastMembersList.Count);
        output.Data.Should().HaveCount(exampleCastMembersList.Count);
        output.Data!.ForEach(outputItem =>
        {
            var item = exampleCastMembersList.Find(x => x.Id == outputItem.Id);

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(outputItem);
        });
    }

    [Trait("EndToEnd/API", "CastMember/ListCastMember - Endpoints")]
    [Fact(DisplayName = nameof(ListWhenIsEmpty))]
    public async Task ListWhenIsEmpty()
    {
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/cast_members");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(0);
        output.Meta!.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(0);
    }

    [Trait("EndToEnd/API", "CastMember/ListCastMember - Endpoints")]
    [Theory(DisplayName = nameof(ListPaginated))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
    )
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(quantityCategoriesToGenerate);
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var input = new ListCastMembersInput(
            page,
            perPage,
            "",
            "",
            SearchOrder.ASC
        );

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/cast_members", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(exampleCastMembersList.Count);
        output.Data!.ForEach(outputItem =>
        {
            var item = exampleCastMembersList.Find(x => x.Id == outputItem.Id);

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(outputItem);
        });
    }

    [Trait("EndToEnd/API", "CastMember/ListCastMember - Endpoints")]
    [Theory(DisplayName = nameof(SearchByText))]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Sci-fi", 1, 5, 5, 6)]
    [InlineData("Sci-fi", 1, 2, 2, 6)]
    [InlineData("Sci-fi", 2, 5, 1, 6)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
    )
    {
        var exampleCastMemberList = _fixture.GetExampleCastMembersListWithNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Future",
            "Sci-fi",
            "Sci-fi Robots",
            "Sci-fi StarWars",
            "Sci-fi StarTrek"
        });
        await _fixture.Persistence.InsertListAsync(exampleCastMemberList);
        var input = new ListCastMembersInput(
            page,
            perPage,
            search,
            "",
            SearchOrder.ASC
        );

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/cast_members", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data!.ForEach(outputItem =>
        {
            var item = exampleCastMemberList.Find(x => x.Id == outputItem.Id);

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(outputItem);
        });
    }

    [Trait("EndToEnd/API", "CastMember/ListCastMember - Endpoints")]
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderby,
        string order
    )
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(10);
        await _fixture.Persistence.InsertListAsync(exampleCastMembersList);
        var searchOrder = order == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var input = new ListCastMembersInput(
            1,
            10,
            "",
            orderby,
            searchOrder
        );

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/cast_members", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(exampleCastMembersList.Count);
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(exampleCastMembersList.Count);

        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(
           exampleCastMembersList,
           orderby,
           searchOrder
       );

        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Data![i];

            expectedItem.Should().NotBeNull();
            outPutItem.Should().NotBeNull();
            outPutItem.Should().BeEquivalentTo(expectedItem);
        }
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
