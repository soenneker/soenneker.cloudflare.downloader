using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Playwright.Installation.Abstract;
using Soenneker.Playwrights.Extensions.Stealth;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.Downloader.Results;
using Soenneker.Cloudflare.Downloader.Requests;
using Soenneker.Extensions.String;

namespace Soenneker.Cloudflare.Downloader;

/// <summary>
/// Browser-backed downloader for retrieving rendered page content and files, including from Cloudflare-protected or JS-heavy sites.
/// </summary>
public sealed class CloudflareDownloader : ICloudflareDownloader
{
    private const int _defaultTimeoutMs = 60_000;
    private const int _defaultPostNavigationDelayMs = 1_500;

    private static readonly Regex _contentDispositionFilenameRegex = new(@"filename\*?=(?:UTF-8'')?[""]?([^"";\r\n]+)[""]?|filename=[""]?([^"";\r\n]+)[""]?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly ILogger<CloudflareDownloader> _logger;
    private readonly IPlaywrightInstallationUtil _playwrightInstallationUtil;

    public CloudflareDownloader(ILogger<CloudflareDownloader> logger, IPlaywrightInstallationUtil playwrightInstallationUtil)
    {
        _logger = logger;
        _playwrightInstallationUtil = playwrightInstallationUtil;
    }

    public async ValueTask<CloudflareDownloadResult> DownloadPage(CloudflareDownloadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Url.IsNullOrWhiteSpace())
            throw new ArgumentException("URL cannot be null or whitespace.", nameof(request));

        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out Uri? uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("URL must be a valid absolute HTTP or HTTPS URL.", nameof(request));
        }

        _logger.LogInformation("Starting browser download for {Url}", request.Url);

        await _playwrightInstallationUtil.EnsureInstalled(cancellationToken)
                                         .NoSync();

        try
        {
            using IPlaywright playwright = await Microsoft.Playwright.Playwright.CreateAsync()
                                                          .NoSync();

            await using IBrowser browser = await playwright.LaunchStealthChromium()
                                                           .NoSync();

            await using IBrowserContext context = await browser.CreateStealthContext()
                                                               .NoSync();

            IPage page = await context.NewPageAsync()
                                      .NoSync();

            try
            {
                page.SetDefaultNavigationTimeout(request.TimeoutMs);
                page.SetDefaultTimeout(request.TimeoutMs);

                _logger.LogDebug("Navigating to {Url} with wait strategy {WaitUntil}", request.Url, request.WaitUntil);

                IResponse? response = await page.GotoAsync(request.Url, new PageGotoOptions
                                                {
                                                    Timeout = request.TimeoutMs,
                                                    WaitUntil = request.WaitUntil
                                                })
                                                .NoSync();

                if (request.PostNavigationDelayMs > 0)
                    await page.WaitForTimeoutAsync(request.PostNavigationDelayMs)
                              .NoSync();

                if (!string.IsNullOrWhiteSpace(request.WaitForSelector))
                {
                    _logger.LogDebug("Waiting for selector {Selector}", request.WaitForSelector);

                    await page.WaitForSelectorAsync(request.WaitForSelector, new PageWaitForSelectorOptions
                              {
                                  Timeout = request.TimeoutMs,
                                  State = request.WaitForSelectorState
                              })
                              .NoSync();
                }

                string html = request.IncludeHtml
                    ? await page.ContentAsync()
                                .NoSync()
                    : string.Empty;

                string text = request.IncludeText
                    ? await page.InnerTextAsync("body")
                                .NoSync()
                    : string.Empty;

                string? title = request.IncludeTitle
                    ? await page.TitleAsync()
                                .NoSync()
                    : null;

                byte[]? screenshot = null;

                if (request.IncludeScreenshot)
                {
                    screenshot = await page.ScreenshotAsync(new PageScreenshotOptions
                                           {
                                               FullPage = request.FullPageScreenshot,
                                               Type = ScreenshotType.Png
                                           })
                                           .NoSync();
                }

                string finalUrl = page.Url;
                int? statusCode = response?.Status;
                bool success = response?.Ok ?? false;
                string? contentType = GetContentType(response);

                _logger.LogInformation(
                    "Completed browser download for {Url}. Success: {Success}, Status: {StatusCode}, FinalUrl: {FinalUrl}, HtmlLength: {HtmlLength}",
                    request.Url, success, statusCode, finalUrl, html.Length);

                return new CloudflareDownloadResult
                {
                    RequestedUrl = request.Url,
                    FinalUrl = finalUrl,
                    Success = success,
                    StatusCode = statusCode,
                    StatusText = response?.StatusText,
                    Title = title,
                    Html = html,
                    Text = text,
                    ContentType = contentType,
                    Screenshot = screenshot,
                    ErrorMessage = null
                };
            }
            finally
            {
                await page.CloseAsync()
                          .NoSync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading page from {Url}", request.Url);

            return new CloudflareDownloadResult
            {
                RequestedUrl = request.Url,
                FinalUrl = null,
                Success = false,
                StatusCode = null,
                StatusText = null,
                Title = null,
                Html = string.Empty,
                Text = string.Empty,
                ContentType = null,
                Screenshot = null,
                ErrorMessage = ex.Message,
                Exception = ex
            };
        }
    }

    public async ValueTask<string?> DownloadHtml(string url, int timeoutMs = _defaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        CloudflareDownloadResult result = await DownloadPage(new CloudflareDownloadRequest
            {
                Url = url,
                TimeoutMs = timeoutMs,
                IncludeHtml = true,
                IncludeText = false,
                IncludeTitle = false,
                WaitUntil = WaitUntilState.DOMContentLoaded,
                PostNavigationDelayMs = _defaultPostNavigationDelayMs
            }, cancellationToken)
            .NoSync();

        return result.Success ? result.Html : null;
    }

    public async ValueTask<CloudflareFileDownloadResult> DownloadFile(CloudflareFileDownloadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Url.IsNullOrWhiteSpace())
            throw new ArgumentException("URL cannot be null or whitespace.", nameof(request));

        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out Uri? uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("URL must be a valid absolute HTTP or HTTPS URL.", nameof(request));
        }

        _logger.LogInformation("Starting file download for {Url}", request.Url);

        await _playwrightInstallationUtil.EnsureInstalled(cancellationToken)
                                         .NoSync();

        try
        {
            using IPlaywright playwright = await Microsoft.Playwright.Playwright.CreateAsync()
                                                          .NoSync();

            await using IBrowser browser = await playwright.LaunchStealthChromium()
                                                           .NoSync();

            await using IBrowserContext context = await browser.CreateStealthContext()
                                                               .NoSync();

            IPage page = await context.NewPageAsync()
                                      .NoSync();

            try
            {
                page.SetDefaultNavigationTimeout(request.TimeoutMs);
                page.SetDefaultTimeout(request.TimeoutMs);

                IResponse? response = await page.GotoAsync(request.Url, new PageGotoOptions
                                                {
                                                    Timeout = request.TimeoutMs,
                                                    WaitUntil = WaitUntilState.Load
                                                })
                                                .NoSync();

                if (response == null)
                {
                    return new CloudflareFileDownloadResult
                    {
                        RequestedUrl = request.Url,
                        FinalUrl = page.Url,
                        Success = false,
                        ErrorMessage = "No response received."
                    };
                }

                byte[] body = await response.BodyAsync();
                string? contentType = GetContentType(response);
                string? suggestedFileName = GetSuggestedFileName(response);

                _logger.LogInformation("Completed file download for {Url}. Success: {Success}, Status: {StatusCode}, Size: {Size}", request.Url, response.Ok,
                    response.Status, body.Length);

                return new CloudflareFileDownloadResult
                {
                    RequestedUrl = request.Url,
                    FinalUrl = response.Url,
                    Success = response.Ok,
                    StatusCode = response.Status,
                    StatusText = response.StatusText,
                    ContentType = contentType,
                    SuggestedFileName = suggestedFileName,
                    Data = body,
                    ErrorMessage = response.Ok ? null : response.StatusText
                };
            }
            finally
            {
                await page.CloseAsync()
                          .NoSync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file from {Url}", request.Url);

            return new CloudflareFileDownloadResult
            {
                RequestedUrl = request.Url,
                FinalUrl = null,
                Success = false,
                StatusCode = null,
                StatusText = null,
                ContentType = null,
                SuggestedFileName = null,
                Data = null,
                ErrorMessage = ex.Message,
                Exception = ex
            };
        }
    }

    public async ValueTask<string?> DownloadFile(string url, int timeoutMs = _defaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        CloudflareFileDownloadResult result = await DownloadFile(new CloudflareFileDownloadRequest
            {
                Url = url,
                TimeoutMs = timeoutMs
            }, cancellationToken)
            .NoSync();

        if (!result.Success || result.Data == null)
            return null;

        return System.Text.Encoding.UTF8.GetString(result.Data);
    }

    public async ValueTask<string?> GetPageContent(string url, int timeoutMs = _defaultTimeoutMs, CancellationToken cancellationToken = default)
    {
        return await DownloadHtml(url, timeoutMs, cancellationToken)
            .NoSync();
    }

    private static string? GetContentType(IResponse? response)
    {
        if (response?.Headers == null)
            return null;
        try
        {
            return response.Headers.GetValueOrDefault("content-type");
        }
        catch
        {
            return null;
        }
    }

    private static string? GetSuggestedFileName(IResponse response)
    {
        if (response.Headers == null)
            return null;
        try
        {
            if (!response.Headers.TryGetValue("content-disposition", out string? disposition) || string.IsNullOrWhiteSpace(disposition))
                return null;

            Match m = _contentDispositionFilenameRegex.Match(disposition);
            if (!m.Success)
                return null;

            string? name = m.Groups[1].Success
                ? m.Groups[1]
                   .Value.Trim()
                : m.Groups[2]
                   .Value.Trim();
            return string.IsNullOrWhiteSpace(name) ? null : name;
        }
        catch
        {
            return null;
        }
    }
}