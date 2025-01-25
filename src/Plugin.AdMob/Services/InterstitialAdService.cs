using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

public interface IInterstitialAdService
{
    IInterstitialAd CreateAd(string adUnitId = null);

    void PrepareAd(string adUnitId = null);

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
