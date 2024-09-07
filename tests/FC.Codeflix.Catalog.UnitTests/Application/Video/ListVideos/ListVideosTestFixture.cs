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
}
