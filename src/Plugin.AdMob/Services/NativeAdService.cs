using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing interstitial ads.
/// </summary>
public interface INativeAdService
{

}

internal class NativeAdService(IAdConsentService _adConsentService) 
    : INativeAdService
{
    private static string? GetAdUnitId(string? adUnitId)
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Interstitial;
        }

        return adUnitId ?? AdConfig.DefaultInterstitialAdUnitId;
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
