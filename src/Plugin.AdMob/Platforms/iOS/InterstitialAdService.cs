using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Configuration;
using UIKit;

namespace Plugin.AdMob.Services;

internal partial class InterstitialAdService : IInterstitialAdService
{
    private InterstitialAd _ad;

    public void PrepareAd(string adUnitId)
    {
        if (string.IsNullOrEmpty(adUnitId) && !string.IsNullOrEmpty(AdConfig.DefaultInterstitialAdUnitId))
        {
            adUnitId = AdConfig.DefaultInterstitialAdUnitId;
        }

        if (AdConfig.UseTestAdUnitIds)
        {
            adUnitId = AdMobTestAdUnits.Interstitial;
        }

        var request = Request.GetDefaultRequest();
        InterstitialAd.Load(adUnitId, request, OnAdLoaded);
    }

    private void OnAdLoaded(InterstitialAd interstitialAd, NSError error)
    {
        _ad = interstitialAd;
    }

    public void PrepareAd()
    {
        PrepareAd(null);
    }

    public void ShowAd()
    {
        if (_ad is null)
        {
            return;
        }

        var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
        _ad.Present(viewController);

        PrepareAd(null);
    }
}
