using Plugin.AdMob.Configuration;
using Plugin.AdMob.Handlers;
using Plugin.AdMob.Services;

namespace Plugin.AdMob;

/// <summary>
/// Handles plugin initialization.
/// </summary>
public static class Config
{
    /// <summary>
    /// Registers the AdMob plugin's services and view handlers.
    /// </summary>
    /// <param name="builder">The MAUI application builder.</param>
    /// <param name="androidDefaultBannerAdUnitId">Optional banner ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultBannerAdUnitId" />.</param>
    /// <param name="androidDefaultInterstitialAdUnitId">Optional interstitial ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultInterstitialAdUnitId" />.</param>
    /// <param name="androidDefaultRewardedAdUnitId">Optional rewarded ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultRewardedAdUnitId" />.</param>
    /// <param name="iosDefaultBannerAdUnitId">Optional banner ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultBannerAdUnitId" />.</param>
    /// <param name="iosDefaultInterstitialAdUnitId">Optional interstitial ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultInterstitialAdUnitId" />.</param>
    /// <param name="iosDefaultRewardedAdUnitId">Optional rewarded ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultRewardedAdUnitId" />.</param>
    /// <returns>The source MAUI application builder.</returns>
    public static MauiAppBuilder UseAdMob(
        this MauiAppBuilder builder, 
        string androidDefaultBannerAdUnitId = null,
        string androidDefaultInterstitialAdUnitId = null,
        string androidDefaultRewardedAdUnitId = null,
        string iosDefaultBannerAdUnitId = null,
        string iosDefaultInterstitialAdUnitId = null,
        string iosDefaultRewardedAdUnitId = null)
    {
#if ANDROID
        AdConfig.DefaultBannerAdUnitId = androidDefaultBannerAdUnitId;
        AdConfig.DefaultInterstitialAdUnitId = androidDefaultInterstitialAdUnitId;
        AdConfig.DefaultRewardedAdUnitId = androidDefaultRewardedAdUnitId;
#elif IOS
        AdConfig.DefaultBannerAdUnitId = iosDefaultBannerAdUnitId;
        AdConfig.DefaultInterstitialAdUnitId = iosDefaultInterstitialAdUnitId;
        AdConfig.DefaultRewardedAdUnitId = iosDefaultRewardedAdUnitId;
#endif

        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(BannerAd), typeof(BannerAdHandler));
        });

        builder.Services.AddSingleton<IInterstitialAdService, InterstitialAdService>();
        builder.Services.AddSingleton<IRewardedAdService, RewardedAdService>();

        return builder;
    }
}
