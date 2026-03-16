using System;

namespace Soenneker.Cloudflare.Downloader.Results;

/// <summary>
/// Result of a file download (raw response body bytes).
/// </summary>
public sealed class CloudflareFileDownloadResult
{
    /// <summary>
    /// The URL that was requested (as passed in the request).
    /// </summary>
    public required string RequestedUrl { get; set; }

    /// <summary>
    /// The final URL after any redirects. Null if the request failed before a response was received.
    /// </summary>
    public string? FinalUrl { get; set; }

    /// <summary>
    /// True when the HTTP response was successful (status in the 2xx range).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// HTTP status code of the response (e.g. 200, 404). Null if no response was received.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// HTTP status text of the response (e.g. "OK", "Not Found"). Null if no response was received.
    /// </summary>
    public string? StatusText { get; set; }

    /// <summary>
    /// Value of the response Content-Type header, when available.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Suggested filename from the Content-Disposition header, when present. Null if not provided.
    /// </summary>
    public string? SuggestedFileName { get; set; }

    /// <summary>
    /// Raw response body bytes. Null when the request failed or no body was received.
    /// </summary>
    public byte[]? Data { get; set; }

    /// <summary>
    /// Short error message when the download failed (e.g. exception or status text). Null on success.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The exception that caused the failure, when available. Null on success or when no exception was captured.
    /// </summary>
    public Exception? Exception { get; set; }
}
