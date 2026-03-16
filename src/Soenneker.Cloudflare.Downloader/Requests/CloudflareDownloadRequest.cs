using Microsoft.Playwright;

namespace Soenneker.Cloudflare.Downloader.Requests;

/// <summary>
/// Options for a browser-backed page download (HTML, text, screenshot, etc.).
/// Browser/context defaults (headless, user-agent, viewport, etc.) are handled by the stealth extension.
/// </summary>
public sealed class CloudflareDownloadRequest
{
    /// <summary>
    /// Absolute HTTP or HTTPS URL of the page to download.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Maximum time in milliseconds to wait for navigation and other operations. Default is 60,000 (60 seconds).
    /// </summary>
    public int TimeoutMs { get; set; } = 60_000;

    /// <summary>
    /// When to consider navigation finished (e.g. DOMContentLoaded, Load, NetworkIdle). Default is DOMContentLoaded.
    /// </summary>
    public WaitUntilState WaitUntil { get; set; } = WaitUntilState.DOMContentLoaded;

    /// <summary>
    /// Optional delay in milliseconds to wait after navigation completes, e.g. for client-side rendering. Default is 1,500.
    /// </summary>
    public int PostNavigationDelayMs { get; set; } = 1_500;

    /// <summary>
    /// Optional CSS selector to wait for before capturing content. When set, the download waits until this element matches the specified state.
    /// </summary>
    public string? WaitForSelector { get; set; }

    /// <summary>
    /// State the selector must match (e.g. Visible, Attached). Used only when <see cref="WaitForSelector"/> is set. Default is Visible.
    /// </summary>
    public WaitForSelectorState WaitForSelectorState { get; set; } = WaitForSelectorState.Visible;

    /// <summary>
    /// When true, the result includes the full HTML of the page. Default is true.
    /// </summary>
    public bool IncludeHtml { get; set; } = true;

    /// <summary>
    /// When true, the result includes the inner text of the body element. Default is false.
    /// </summary>
    public bool IncludeText { get; set; }

    /// <summary>
    /// When true, the result includes the page title. Default is true.
    /// </summary>
    public bool IncludeTitle { get; set; } = true;

    /// <summary>
    /// When true, the result includes a PNG screenshot of the page. Default is false.
    /// </summary>
    public bool IncludeScreenshot { get; set; }

    /// <summary>
    /// When true and <see cref="IncludeScreenshot"/> is true, the screenshot covers the full scrollable page. Default is true.
    /// </summary>
    public bool FullPageScreenshot { get; set; } = true;
}
