using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture> { }

public class ListVideosTestFixture : VideoUseCaseBaseFixture
{
    public List<DomainEntity.Video> VideosList(int? quantity = 10) 
        => Enumerable
            .Range(1, Random.Shared.Next(2, quantity ?? 10))
            .Select(_ => GetValidVideoAllProperties())
            .ToList();
}
