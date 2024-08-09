using FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedia;

[CollectionDefinition(nameof(UploadMediasTestFixture))]
public class UploadMediaTestFixtureCollection : ICollectionFixture<UploadMediasTestFixture> { }

public class UploadMediasTestFixture : VideoUseCaseBaseFixture
{
    public UploadMediasInput GetValidInput(Guid? videoId = null, bool withVideoFile = true, bool withTrailerFile = true)
    => new(
            videoId ?? Guid.NewGuid(),
            withVideoFile ? GetValidMediaFileInput() : null,
            withTrailerFile ? GetValidMediaFileInput() : null
    );
}
