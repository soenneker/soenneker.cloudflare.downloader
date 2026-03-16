using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.Downloader.Requests;
using Soenneker.Cloudflare.Downloader.Results;

namespace Soenneker.Cloudflare.Downloader.Abstract;

/// <summary>
/// Browser-backed downloader for retrieving rendered page content and files from Cloudflare-protected or JS-heavy sites.
/// </summary>
public interface ICloudflareDownloader
{
    /// <summary>
    /// Downloads a full page with configurable options (HTML, text, title, screenshot, etc.).
    /// </summary>
    /// <param name="request">Request options (URL, timeouts, what to include, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with success, HTML, text, screenshot, and metadata</returns>
    [Pure]
    ValueTask<CloudflareDownloadResult> DownloadPage(CloudflareDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convenience method: downloads a page and returns HTML only. Uses DOMContentLoaded and a short post-navigation delay.
    /// </summary>
    /// <param name="url">Page URL</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>HTML string if successful, otherwise null</returns>
    [Pure]
    ValueTask<string?> DownloadHtml(string url, int timeoutMs = 60_000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file (binary or text) from the given URL using the browser, e.g. for Cloudflare-protected file URLs.
    /// </summary>
    /// <param name="request">Request options (URL, timeout, headers, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with raw response body bytes, content type, and optional suggested filename</returns>
    [Pure]
    ValueTask<CloudflareFileDownloadResult> DownloadFile(CloudflareFileDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the given URL and returns the response body as a string (e.g. for JSON, XML, or text files behind Cloudflare).
    /// </summary>
    /// <param name="url">The URL of the file to download</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response body as a string if successful, otherwise null</returns>
    [Pure]
    ValueTask<string?> DownloadFile(string url, int timeoutMs = 60_000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches text-based content from a URL using Playwright (legacy convenience). Prefer <see cref="DownloadHtml"/> or <see cref="DownloadPage"/> for new code.
    /// </summary>
    /// <param name="url">The direct URL to the resource</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The raw content as a string, or null on failure</returns>
    [Pure]
    ValueTask<string?> GetPageContent(string url, int timeoutMs = 60_000, CancellationToken cancellationToken = default);
}