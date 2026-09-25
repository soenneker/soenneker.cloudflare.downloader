using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soenneker.Cloudflare.Downloader;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(JsonDocument))]
internal partial class FormattingJsonContext : JsonSerializerContext;
