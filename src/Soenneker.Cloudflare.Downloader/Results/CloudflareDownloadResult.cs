using System;

namespace Soenneker.Cloudflare.Downloader.Results;

/// <summary>
/// Result of a browser-backed page download.
/// </summary>
public sealed class CloudflareDownloadResult
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
    /// Document title of the page, when requested. Null if not requested or unavailable.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Full HTML source of the page. Empty when not requested or when the request failed.
    /// </summary>
    public string Html { get; set; } = string.Empty;

    /// <summary>
    /// Inner text of the body element. Empty when not requested or when the request failed.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Value of the response Content-Type header, when available.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// PNG screenshot bytes when requested. Null when not requested or when the request failed.
    /// </summary>
    public byte[]? Screenshot { get; set; }

    /// <summary>
    /// Short error message when the download failed (e.g. exception message). Null on success.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The exception that caused the failure, when available. Null on success or when no exception was captured.
    /// </summary>
    public Exception? Exception { get; set; }
}
