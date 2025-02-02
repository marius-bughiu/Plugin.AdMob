using Android.Gms.Ads;
using InterstitialAdLoadCallback = Android.Gms.Ads.Workarounds.InterstitialAdLoadCallback;

namespace Plugin.AdMob.Platforms.Android.Interstitial;

internal class InterstitialAdCallbacks : InterstitialAdLoadCallback
{
    public event EventHandler<global::Android.Gms.Ads.Interstitial.InterstitialAd>? WhenAdLoaded;
    public event EventHandler<LoadAdError>? WhenAdFailedToLoaded;

    public override void OnAdLoaded(global::Android.Gms.Ads.Interstitial.InterstitialAd interstitialAd)
    {
        base.OnAdLoaded(interstitialAd);
        WhenAdLoaded?.Invoke(this, interstitialAd);
    }

    public override void OnAdFailedToLoad(LoadAdError error)
    {
        base.OnAdFailedToLoad(error);
        WhenAdFailedToLoaded?.Invoke(this, error);
    }
}
