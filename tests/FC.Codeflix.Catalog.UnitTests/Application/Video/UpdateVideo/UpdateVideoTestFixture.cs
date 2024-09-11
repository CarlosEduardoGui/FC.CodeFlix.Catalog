using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture> { }

public class UpdateVideoTestFixture : VideoUseCaseBaseFixture
{
    public UpdateVideoInput CreateValidInput(Guid Id)
    => new(
        Id,
        GetValidTitle(),
        GetValidDescription(),
        GetRandomBoolean(),
        GetValidDuration(),
        GetRandomRating(),
        GetValidYearLauched(),
        GetRandomBoolean()
    );
}
