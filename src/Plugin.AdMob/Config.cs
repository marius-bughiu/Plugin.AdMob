using Microsoft.Maui.LifecycleEvents;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Handlers;
using Plugin.AdMob.Services;

namespace Plugin.AdMob;

/// <summary>
/// Handles plugin initialization.
/// </summary>
public static class Config
{
    private static bool _initialized;

    /// <summary>
    /// Registers the AdMob plugin's services and view handlers.
    /// </summary>
    /// <param name="builder">The MAUI application builder.</param>
    /// <param name="androidDefaultBannerAdUnitId">Optional banner ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultBannerAdUnitId" />.</param>
    /// <param name="androidDefaultInterstitialAdUnitId">Optional interstitial ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultInterstitialAdUnitId" />.</param>
    /// <param name="iosDefaultBannerAdUnitId">Optional banner ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultBannerAdUnitId" />.</param>
    /// <param name="iosDefaultInterstitialAdUnitId">Optional interstitial ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultInterstitialAdUnitId" />.</param>
    /// <param name="androidDefaultRewardedAdUnitId">Optional rewarded ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultRewardedAdUnitId" />.</param>
    /// <param name="androidDefaultRewardedInterstitialAdUnitId">Optional interstitial rewarded ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultRewardedInterstitialAdUnitId" />.</param>
    /// <param name="iosDefaultRewardedAdUnitId">Optional rewarded ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultRewardedAdUnitId" />.</param>
    /// <param name="iosDefaultRewardedInterstitialAdUnitId">Optional interstitial rewarded ad unit ID to be used by default on iOS. See <see cref="AdConfig.DefaultRewardedInterstitialAdUnitId" />.</param>
    /// <param name="androidDefaultAppOpenAdUnitId">Optional app open ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultAppOpenAdUnitId" />.</param>
    /// <param name="iosDefaultAppOpenAdUnitId">Optional app open ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultAppOpenAdUnitId" />.</param>
    /// <param name="androidDefaultNativeAdUnitId">Optional native ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultNativeAdUnitId" />.</param>
    /// <param name="iosDefaultNativeAdUnitId">Optional native ad unit ID to be used by default on Android. See <see cref="AdConfig.DefaultNativeAdUnitId" />.</param>
    /// <param name="disableConsentCheck">Default is false. When set to true, the plugin will no longer check if there is consent before making an ad request. You should use <see cref="IAdConsentService" /> to ask for consent and ensure you can request ads before making any ad request. If consent is missing, the Google Ads SDK will automatically drop the ad requests and no ads will be served.</param>
    /// <param name="automaticallyAskForConsent">Default is true. When set to false, the plugin will no longer ask for consent automatically. Use <see cref="IAdConsentService" /> to manage consent.</param>
    /// <returns>The source MAUI application builder.</returns>
    public static MauiAppBuilder UseAdMob(
        this MauiAppBuilder builder,
        string? androidDefaultBannerAdUnitId = null,
        string? androidDefaultInterstitialAdUnitId = null,
        string? iosDefaultBannerAdUnitId = null,
        string? iosDefaultInterstitialAdUnitId = null,
        string? androidDefaultRewardedAdUnitId = null,
        string? androidDefaultRewardedInterstitialAdUnitId = null,
        string? iosDefaultRewardedAdUnitId = null,
        string? iosDefaultRewardedInterstitialAdUnitId = null,
        string? androidDefaultAppOpenAdUnitId = null,
        string? iosDefaultAppOpenAdUnitId = null,
        string? androidDefaultNativeAdUnitId = null,
        string? iosDefaultNativeAdUnitId = null,
        bool disableConsentCheck = false,
        bool automaticallyAskForConsent = true)
    {
#if ANDROID
        AdConfig.DefaultBannerAdUnitId = androidDefaultBannerAdUnitId;
        AdConfig.DefaultInterstitialAdUnitId = androidDefaultInterstitialAdUnitId;
        AdConfig.DefaultRewardedAdUnitId = androidDefaultRewardedAdUnitId;
        AdConfig.DefaultRewardedInterstitialAdUnitId = androidDefaultRewardedInterstitialAdUnitId;
        AdConfig.DefaultAppOpenAdUnitId = androidDefaultAppOpenAdUnitId;
        AdConfig.DefaultNativeAdUnitId = androidDefaultNativeAdUnitId;
#elif IOS
        AdConfig.DefaultBannerAdUnitId = iosDefaultBannerAdUnitId;
        AdConfig.DefaultInterstitialAdUnitId = iosDefaultInterstitialAdUnitId;
        AdConfig.DefaultRewardedAdUnitId = iosDefaultRewardedAdUnitId;
        AdConfig.DefaultRewardedInterstitialAdUnitId = iosDefaultRewardedInterstitialAdUnitId;
        AdConfig.DefaultAppOpenAdUnitId = iosDefaultAppOpenAdUnitId;
        AdConfig.DefaultNativeAdUnitId = iosDefaultNativeAdUnitId;
#endif

        AdConfig.DisableConsentCheck = disableConsentCheck;

        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(BannerAd), typeof(BannerAdHandler));
            handlers.AddHandler(typeof(NativeAdView), typeof(NativeAdHandler));
        });

        builder.Services.AddSingleton<IInterstitialAdService, InterstitialAdService>();
        builder.Services.AddSingleton<IRewardedAdService, RewardedAdService>();
        builder.Services.AddSingleton<IRewardedInterstitialAdService, RewardedInterstitialAdService>();
        builder.Services.AddSingleton<IAppOpenAdService, AppOpenAdService>();
        builder.Services.AddSingleton<INativeAdService, NativeAdService>();

        IAdConsentService adConsentService = new AdConsentService();
        builder.Services.AddSingleton<IAdConsentService>(adConsentService);

#if ANDROID || IOS
        if (automaticallyAskForConsent is true)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnStart(_ => OnStart()));
#elif IOS
                events.AddiOS(ios => ios.OnActivated(_ => OnStart()));
#endif
            });
        }

        void OnStart()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            adConsentService.LoadAndShowConsentFormIfRequired();
        }
#endif

        return builder;
    }

    /// <summary>
    /// Configures the consent debug settings to be used during testing.
    /// </summary>
    /// <param name="builder">The MAUI application builder.</param>
    /// <param name="consentDebugSettings">The consent debug settings.</param>
    /// <returns>The source MAUI application builder.</returns>
    public static MauiAppBuilder UseConsentDebugSettings(
        this MauiAppBuilder builder,
        ConsentDebugSettings consentDebugSettings
        )
    {
        ConsentDebugSettings.Current = consentDebugSettings;

        return builder;
    }
}
