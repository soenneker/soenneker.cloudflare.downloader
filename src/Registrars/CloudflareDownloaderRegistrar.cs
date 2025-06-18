using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.Downloader.Abstract;

namespace Soenneker.Cloudflare.Downloader.Registrars;

/// <summary>
/// Allows for navigating and downloading from Cloudflare sites in under-attack mode
/// </summary>
public static class CloudflareDownloaderRegistrar
{
    /// <summary>
    /// Adds <see cref="ICloudflareDownloader"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareDownloaderAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<ICloudflareDownloader, CloudflareDownloader>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareDownloader"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareDownloaderAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<ICloudflareDownloader, CloudflareDownloader>();

        return services;
    }
}
