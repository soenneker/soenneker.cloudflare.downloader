[![](https://img.shields.io/nuget/v/soenneker.cloudflare.downloader.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.downloader/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.downloader/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.downloader/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cloudflare.downloader.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.downloader/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.downloader/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.downloader/actions/workflows/codeql.yml)

# Soenneker.Cloudflare.Downloader

A Playwright-backed downloader for rendered pages and files on JavaScript-heavy or Cloudflare-protected sites.

## Installation

```bash
dotnet add package Soenneker.Cloudflare.Downloader
```

## Registration

```csharp
using Soenneker.Cloudflare.Downloader.Registrars;

services.AddCloudflareDownloaderAsSingleton();
```

The first operation ensures the required Playwright browser is installed. The process therefore needs permission to install or access that browser runtime. Scoped registration is also available.

## Downloading rendered content

```csharp
using Microsoft.Playwright;
using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Cloudflare.Downloader.Requests;
using Soenneker.Cloudflare.Downloader.Results;

CloudflareDownloadResult result = await downloader.DownloadPage(
    new CloudflareDownloadRequest
    {
        Url = "https://example.com/catalog",
        WaitUntil = WaitUntilState.DOMContentLoaded,
        WaitForSelector = "main",
        IncludeHtml = true,
        IncludeText = true,
        IncludeTitle = true
    },
    cancellationToken);

if (!result.Success)
    logger.LogWarning("Download failed: {Error}", result.ErrorMessage);
```

The result contains the final URL, response status, optional HTML/text/title, and optional PNG screenshot. An HTTP non-success response sets `Success` to `false`; operational exceptions are captured in the result. Caller-requested cancellation is propagated.

## Downloading files

```csharp
CloudflareFileDownloadResult result = await downloader.DownloadFileToPath(
    "https://example.com/export.zip",
    outputPath,
    cancellationToken: cancellationToken);
```

File responses are fully buffered into `CloudflareFileDownloadResult.Data`, even when also written to disk. Do not use this API for unbounded or untrusted large downloads without enforcing size limits outside the library. `DownloadFile(string)` decodes arbitrary response bytes as UTF-8; use the byte-returning overload for binary content.

## Security and operational notes

Only automate sites and downloads you are authorized to access. Stealth browser settings do not guarantee that a Cloudflare challenge will be solved, and challenge behavior can change independently of this package.

Treat caller-provided URLs as an SSRF boundary in server applications. Restrict destinations before calling the downloader, and never expose arbitrary `FilePath` values to untrusted callers because successful downloads can create parent directories and overwrite the selected path.
