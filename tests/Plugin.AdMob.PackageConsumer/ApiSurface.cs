using Microsoft.Maui.Hosting;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.PackageConsumer;

/// <summary>
/// Touches the published package's public API on every target framework so the build smoke
/// fails if a released version drops, renames, or changes the signature of anything a real
/// consumer depends on. Nothing here runs — it only has to compile and link.
/// </summary>
public static class ApiSurface
{
    public static MauiAppBuilder Configure(MauiAppBuilder builder)
    {
        // The one-line consumer entry point.
        builder.UseAdMob();

        // Global configuration surface.
        AdConfig.UseTestAdUnitIds = true;
        AdConfig.DisableConsentCheck = true;
        AdConfig.AddTestDevice("test-device");

        return builder;
    }

    // Reference the public ad types / service contracts so they must exist and keep their shape.
    public static readonly Type[] PublicSurface =
    [
        typeof(BannerAd),
        typeof(NativeAdView),
        typeof(MediaView),
        typeof(IInterstitialAdService),
        typeof(IRewardedAdService),
        typeof(IRewardedInterstitialAdService),
        typeof(IAppOpenAdService),
        typeof(INativeAdService),
        typeof(IAdConsentService),
        typeof(IInterstitialAd),
        typeof(IRewardedAd),
        typeof(IRewardedInterstitialAd),
        typeof(IAppOpenAd),
        typeof(INativeAd),
        typeof(IAdError),
        typeof(VideoOptions),
    ];
}
