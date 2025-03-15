using Plugin.AdMob.Services;

namespace Plugin.AdMob.Configuration;

/// <summary>
/// Ad configuration.
/// </summary>
public static class AdConfig
{
    /// <summary>
    /// Optional banner ad unit ID to be used by default.
    /// </summary>
    public static string? DefaultBannerAdUnitId { get; set; }

    /// <summary>
    /// Optional interstitial ad unit ID to be used by default.
    /// </summary>
    public static string? DefaultInterstitialAdUnitId { get; set; }

    /// <summary>
    /// Optional rewarded ad unit ID to be used by default
    /// </summary>
    public static string? DefaultRewardedAdUnitId { get; set; }

    /// <summary>
    /// Optional rewarded interstitial ad unit ID to be used by default
    /// </summary>
    public static string? DefaultRewardedInterstitialAdUnitId { get; set; }

    /// <summary>
    /// Optional app open ad unit ID to be used by default
    /// </summary>
    public static string? DefaultAppOpenAdUnitId { get; set; }

    /// <summary>
    /// Optional native ad unit ID to be used by default
    /// </summary>
    public static string? DefaultNativeAdUnitId { get; set; }

    /// <summary>
    /// A list of test device IDs.
    /// </summary>
    public static IList<string> TestDevices { get; } = [];

    /// <summary>
    /// When set to true, all ad requests will be made using test ad unit IDs. 
    /// </summary>
    public static bool UseTestAdUnitIds { get; set; }

    /// <summary>
    /// When set to true, the plugin will no longer check if there is consent before making an ad request. 
    /// You should <see cref="IAdConsentService" /> to ask for consent and ensure you can request ads before making any ad request. 
    /// If consent is missing, the Google Ads SDK will automatically drop the ad requests and no ads will be served.
    /// </summary>
    public static bool DisableConsentCheck { get; set; }

    /// <summary>
    /// Adds a device ID to the list of test devices.
    /// </summary>
    /// <param name="deviceId">The device ID.</param>
    public static void AddTestDevice(string deviceId)
    {
        TestDevices.Add(deviceId);
    }
}
