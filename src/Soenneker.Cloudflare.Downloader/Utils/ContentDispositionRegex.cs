using System.Text.RegularExpressions;

namespace Soenneker.Cloudflare.Downloader.Utils;

internal static partial class ContentDispositionRegex
{
    [GeneratedRegex(@"filename\*?=(?:UTF-8'')?[""]?([^"";\r\n]+)[""]?|filename=[""]?([^"";\r\n]+)[""]?", RegexOptions.IgnoreCase)]
    public static partial Regex Filename();
}