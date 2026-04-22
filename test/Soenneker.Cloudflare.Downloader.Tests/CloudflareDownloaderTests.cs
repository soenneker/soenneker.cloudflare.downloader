using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Tests.Attributes.Local;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cloudflare.Downloader.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloudflareDownloaderTests : HostedUnitTest
{
    private readonly ICloudflareDownloader _util;

    public CloudflareDownloaderTests(Host host) : base(host)
    {
        _util = Resolve<ICloudflareDownloader>(true);
    }

    [Test]
    public void Default()
    {

    }

    [LocalOnly]
    public async ValueTask GetPageContent_ValidUrl_ReturnsContent()
    {
        const string url = "https://api.weather.gov/openapi.json";
        
        string? content = await _util.DownloadFile(url, cancellationToken: CancellationToken);
        content.Should().NotBeNull();
    }
}
