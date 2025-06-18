using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Cloudflare.Downloader.Tests;

[Collection("Collection")]
public sealed class CloudflareDownloaderTests : FixturedUnitTest
{
    private readonly ICloudflareDownloader _util;

    public CloudflareDownloaderTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ICloudflareDownloader>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
