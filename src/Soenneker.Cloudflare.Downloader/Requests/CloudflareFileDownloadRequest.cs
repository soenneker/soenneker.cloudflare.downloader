namespace Soenneker.Cloudflare.Downloader.Requests;

/// <summary>
/// Options for downloading a file (binary or text) via the browser, e.g. from Cloudflare-protected URLs.
/// Browser/context defaults are handled by the stealth extension.
/// </summary>
public sealed class CloudflareFileDownloadRequest
{
    /// <summary>
    /// Absolute HTTP or HTTPS URL of the file to download.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Maximum time in milliseconds to wait for the response. Default is 60,000 (60 seconds).
    /// </summary>
    public int TimeoutMs { get; set; } = 60_000;

    /// <summary>
    /// Optional full path on disk to write the downloaded file. When set, the response body is written via <see cref="Soenneker.Utils.File.Abstract.IFileUtil"/>.
    /// Parent directory is created if it does not exist.
    /// </summary>
    public string? FilePath { get; set; }
}
