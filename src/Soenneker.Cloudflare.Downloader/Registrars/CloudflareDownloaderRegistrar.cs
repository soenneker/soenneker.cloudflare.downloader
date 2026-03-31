using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.Downloader.Abstract;
using Soenneker.Playwrights.Installation.Registrars;
using Soenneker.Utils.Directory.Registrars;
using Soenneker.Utils.File.Registrars;

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
        services.AddFileUtilAsSingleton()
                .AddDirectoryUtilAsSingleton()
                .AddPlaywrightInstallationUtilAsSingleton()
                .TryAddSingleton<ICloudflareDownloader, CloudflareDownloader>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareDownloader"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareDownloaderAsScoped(this IServiceCollection services)
    {
        services.AddFileUtilAsScoped()
                .AddDirectoryUtilAsScoped()
                .AddPlaywrightInstallationUtilAsSingleton()
                .TryAddScoped<ICloudflareDownloader, CloudflareDownloader>();

        return services;
    }
}