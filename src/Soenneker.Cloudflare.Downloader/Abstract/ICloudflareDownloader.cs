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
    /// When <see cref="CloudflareFileDownloadRequest.FilePath"/> is set, the response body is also written to that path via <see cref="Soenneker.Utils.File.Abstract.IFileUtil"/>.
    /// </summary>
    /// <param name="request">Request options (URL, timeout, optional file path, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with raw response body bytes, content type, and optional suggested filename</returns>
    [Pure]
    ValueTask<CloudflareFileDownloadResult> DownloadFile(CloudflareFileDownloadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the given URL and writes it to the specified file path using <see cref="Soenneker.Utils.File.Abstract.IFileUtil"/>.
    /// Parent directory is created if it does not exist.
    /// </summary>
    /// <param name="url">The URL of the file to download</param>
    /// <param name="filePath">Full path on disk where the file will be written</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with success, status, and metadata; <see cref="CloudflareFileDownloadResult.Data"/> is still populated</returns>
    [Pure]
    ValueTask<CloudflareFileDownloadResult> DownloadFileToPath(string url, string filePath, int timeoutMs = 60_000, CancellationToken cancellationToken = default);

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
    /// Downloads JSON from the given URL and returns the response body as a string.
    /// When <paramref name="formatted"/> is true, the JSON is pretty-printed via <see cref="Soenneker.Utils.Json.JsonUtil.Format"/>.
    /// </summary>
    /// <param name="url">The URL of the JSON resource to download</param>
    /// <param name="formatted">When true, pretty-format the JSON; otherwise return as-is</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The JSON string if successful (optionally formatted), otherwise null</returns>
    [Pure]
    ValueTask<string?> DownloadJson(string url, bool formatted = false, int timeoutMs = 60_000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads JSON from the given URL and writes it to the specified file path using <see cref="Soenneker.Utils.File.Abstract.IFileUtil"/>.
    /// Parent directory is created if it does not exist. When <paramref name="formatted"/> is true, the JSON is pretty-printed via <see cref="Soenneker.Utils.Json.JsonUtil.Format"/>.
    /// </summary>
    /// <param name="url">The URL of the JSON resource to download</param>
    /// <param name="filePath">Full path on disk where the file will be written</param>
    /// <param name="formatted">When true, pretty-format the JSON before writing; otherwise write as-is</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the file was successfully downloaded and written; otherwise false</returns>
    [Pure]
    ValueTask<bool> DownloadJsonToPath(string url, string filePath, bool formatted = false, int timeoutMs = 60_000, CancellationToken cancellationToken = default);
}