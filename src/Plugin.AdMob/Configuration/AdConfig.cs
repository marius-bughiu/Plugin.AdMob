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
    /// Optional native video ad unit ID to be used by default
    /// </summary>
    public static string? DefaultNativeVideoAdUnitId { get; set; }

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

    /// <summary>
    /// Allows you to indicate whether you want Google to treat your content as child-directed when you make an ad request.
    /// </summary>
    public static TagForChildDirectedTreatment TagForChildDirectedTreatment { get; set; } = TagForChildDirectedTreatment.None;

    /// <summary>
    /// Allows you to indicate the treatment for users in the European Economic Area (EEA) under the age of consent.
    /// </summary>
    public static TagForUnderAgeOfConsent TagForUnderAgeOfConsent { get; set; } = TagForUnderAgeOfConsent.None;

    /// <summary>
    /// Sets the maximum ad content rating for your ad requests. AdMob ads returned when this is configured have a content rating at or below that level.
    /// </summary>
    public static MaxAdContentRating MaxAdContentRating { get; set; } = MaxAdContentRating.None;
}
