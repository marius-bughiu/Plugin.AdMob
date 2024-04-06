using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

public interface IInterstitialAdService
{
    IInterstitialAd CreateAd(string adUnitId = null);

    void PrepareAd(string adUnitId = null);

    void ShowAd();
}

internal class InterstitialAdService : IInterstitialAdService
{
    private IInterstitialAd _interstitialAd;

    public IInterstitialAd CreateAd(string adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        return new InterstitialAd(adUnitId);
    }

    public void PrepareAd(string adUnitId = null)
    {
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
        adUnitId ??= AdConfig.DefaultInterstitialAdUnitId;

        if (AdConfig.UseTestAdUnitIds)
        {
            adUnitId = AdMobTestAdUnits.Interstitial;
        }

        return adUnitId;
    }
}
