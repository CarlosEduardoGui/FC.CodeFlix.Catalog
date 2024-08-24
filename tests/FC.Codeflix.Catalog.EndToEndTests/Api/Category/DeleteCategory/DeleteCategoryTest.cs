using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;

[Collection(nameof(CategoryBaseFixture))]
public class DeleteCategoryTest : IDisposable
{
    private readonly CategoryBaseFixture _fixture;

    public DeleteCategoryTest(CategoryBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/Api", "Category/Delete - Endpoints")]
    [Fact(DisplayName = nameof(DeleteCategoryOk))]
    public async Task DeleteCategoryOk()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var exampleCategory = exampleCategoryList[10];

        var (response, output) = await
            _fixture.ApiClient.Delete<object>(
                $"/categories/{exampleCategory.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var persistenceCategory = await _fixture
            .Persistence
            .GetByIdAsync(exampleCategory.Id);
        persistenceCategory.Should().BeNull();
    }

    [Trait("EndToEnd/Api", "Category/Delete - Endpoints")]
    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertListAsync(exampleCategoryList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await
            _fixture.ApiClient.Delete<ProblemDetails>(
                $"/categories/{randomGuid}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output!.Type.Should().Be("NotFound");
        output!.Detail.Should().Be($"Category '{randomGuid}' not found.");
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
