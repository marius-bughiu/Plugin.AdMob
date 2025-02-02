using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing interstitial ads.
/// </summary>
public interface IInterstitialAdService
{
    /// <summary>
    /// Creates an interstitial ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultInterstitialAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>An interstitial ad instance.</returns>
    IInterstitialAd CreateAd(string adUnitId = null);

    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultInterstitialAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    void PrepareAd(string adUnitId = null);

    /// <summary>
    /// Displays the already prepared ad. Does nothing if no ad was prepared.
    /// </summary>
    void ShowAd();
}

internal class InterstitialAdService(IAdConsentService _adConsentService) 
    : IInterstitialAdService
{
    private IInterstitialAd _interstitialAd;

    public IInterstitialAd CreateAd(string adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        return new InterstitialAd(adUnitId);
    }

    public void PrepareAd(string adUnitId = null)
    {
        if (CanRequestAds() is false)
        {
            return;
        }

        var interstitialAd = CreateAd(adUnitId);
        interstitialAd.Load();

        _interstitialAd = interstitialAd;
    }

    public void ShowAd()
    {
        _interstitialAd?.Show();
    }

    private string GetAdUnitId(string adUnitId)
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
