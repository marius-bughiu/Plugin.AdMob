using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing native ads.
/// </summary>
public interface INativeAdService
{
    /// <summary>
    /// Creates a native ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultNativeAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>A native ad instance.</returns>
    INativeAd CreateAd(string? adUnitId = null);
}

internal class NativeAdService(IAdConsentService _adConsentService)
    : INativeAdService
{
    public INativeAd CreateAd(string? adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default native ad unit ID has been configured.");
        }

        return new NativeAd(adUnitId);
    }

    private static string? GetAdUnitId(string? adUnitId)
    {
#if ANDROID || IOS
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Native;
        }
#endif

        return adUnitId ?? AdConfig.DefaultNativeAdUnitId;
    }

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService.CanRequestAds();
    }
}
