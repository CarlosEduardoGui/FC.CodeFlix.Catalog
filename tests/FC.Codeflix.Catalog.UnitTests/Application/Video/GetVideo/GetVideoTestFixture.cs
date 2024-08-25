using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.GetVideo;

[CollectionDefinition(nameof(GetVideoTestFixture))]
public class GetVideoTestFixtureCollection : ICollectionFixture<GetVideoTestFixture> { }

public class GetVideoTestFixture : VideoUseCaseBaseFixture
{
}
