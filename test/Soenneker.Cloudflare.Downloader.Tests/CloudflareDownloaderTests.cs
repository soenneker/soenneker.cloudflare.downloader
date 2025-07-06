using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Facts.Local;
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

    [LocalFact]
    public async ValueTask GetPageContent_ValidUrl_ReturnsContent()
    {
        const string url = "https://api.instantly.ai/openapi/api_v2.json";
        
        string? content = await _util.GetPageContent(url);
        content.Should().NotBeNull();
    }
}
