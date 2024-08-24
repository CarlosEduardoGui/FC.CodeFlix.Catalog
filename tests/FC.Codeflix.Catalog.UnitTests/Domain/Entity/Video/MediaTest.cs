using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Commom;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class MediaTest : BaseFixture
{
    private readonly VideoTestFixture _fixture;

    public MediaTest(VideoTestFixture videoTestFixture)
        => _fixture = videoTestFixture;

    [Trait("Domain", "Media - Entity")]
    [Fact(DisplayName = nameof(InstantiateOk))]
    public void InstantiateOk()
    {
        var expectedFilePath = _fixture.GetValidMediaPath();

        var media = new Media(expectedFilePath);

        media.Should().NotBeNull();
        media.FilePath.Should().Be(expectedFilePath);
        media.EncodedPath.Should().BeNull();
        media.Status.Should().Be(MediaStatus.Pending);
    }

    [Trait("Domain", "Media - Entity")]
    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    public void UpdateAsSentToEncode()
    {
        var media = _fixture.GetValidMedia();

        media.UpdateAsSentToEncode();

        media.Should().NotBeNull();
        media.Status.Should().Be(MediaStatus.Processing);
    }

    [Trait("Domain", "Media - Entity")]
    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    public void UpdateAsEncoded()
    {
        var media = _fixture.GetValidMedia();
        var encodedExamplePath = _fixture.GetValidMediaPath();
        media.UpdateAsSentToEncode();

        media.UpdateAsEncoded(encodedExamplePath);

        media.Should().NotBeNull();
        media.Status.Should().Be(MediaStatus.Completed);
        media.EncodedPath.Should().Be(encodedExamplePath);
    }
}
