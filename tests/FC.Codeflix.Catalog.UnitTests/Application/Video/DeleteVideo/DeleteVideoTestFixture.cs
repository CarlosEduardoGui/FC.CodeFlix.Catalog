using FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.DeleteVideo;

[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture> { }

public class DeleteVideoTestFixture : VideoUseCaseBaseFixture
{
    public DeleteVideoInput GetValidInput(Guid? id = null)
        => new(id ?? Guid.NewGuid());
}
