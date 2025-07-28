using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Playwright.Installation.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Playwrights.Extensions.Stealth;

namespace Soenneker.Cloudflare.Downloader;

///<inheritdoc cref="ICloudflareDownloader"/>
public sealed class CloudflareDownloader : ICloudflareDownloader
{
    private readonly ILogger<CloudflareDownloader> _logger;
    private readonly IPlaywrightInstallationUtil _playwrightInstallationUtil;

    public CloudflareDownloader(ILogger<CloudflareDownloader> logger, IPlaywrightInstallationUtil playwrightInstallationUtil)
    {
        _logger = logger;
        _playwrightInstallationUtil = playwrightInstallationUtil;
    }

    public async ValueTask<string?> GetPageContent(string url, int timeoutMs = 60000, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting GetPageContent from: {Url} ...", url);

        await _playwrightInstallationUtil.EnsureInstalled(cancellationToken).NoSync();

        try
        {
            using IPlaywright playwright = await Microsoft.Playwright.Playwright.CreateAsync().NoSync();

            _logger.LogDebug("Launching headless Chromium browser...");
            await using IBrowser browser = await playwright.LaunchStealthChromium().NoSync();

            IBrowserContext context = await browser.CreateStealthContext().NoSync();

            IPage page = await context.NewPageAsync().NoSync();

            _logger.LogInformation("Navigating to URL ({Url})...", url);
            IResponse? response = await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = timeoutMs
            }).NoSync();

            if (response == null)
            {
                _logger.LogWarning("No response received for URL: {Url}", url);
                return null;
            }

            _logger.LogInformation("Received HTTP {StatusCode} from {Url}", response.Status, url);

            if (!response.Ok)
            {
                _logger.LogWarning("Non-success response: {Status} {StatusText}", response.Status, response.StatusText);
                return null;
            }

            _logger.LogInformation("Successfully fetched content, reading...");

            string content = await response.TextAsync().NoSync();

            _logger.LogInformation("Successfully read content (length: {Length})", content.Length);

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while downloading file from {Url}", url);
            return null;
        }
    }
}