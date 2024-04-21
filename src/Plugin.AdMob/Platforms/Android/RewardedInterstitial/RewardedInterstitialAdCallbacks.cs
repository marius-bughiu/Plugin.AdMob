using Android.Gms.Ads;
using RewardedInterstitialAdLoadCallback = Android.Gms.Ads.Workarounds.RewardedInterstitialAdLoadCallback;

namespace Plugin.AdMob.Platforms.Android.RewardedInterstitial;

internal class RewardedInterstitialAdCallbacks : RewardedInterstitialAdLoadCallback
{
    public event EventHandler<global::Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd> WhenAdLoaded;
    public event EventHandler<LoadAdError> WhenAdFailedToLoaded;

    public override void OnAdLoaded(global::Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd rewardedInterstitialAd)
    {
        base.OnAdLoaded(rewardedInterstitialAd);
        WhenAdLoaded?.Invoke(this, rewardedInterstitialAd);
    }

    public override void OnAdFailedToLoad(LoadAdError error)
    {
        base.OnAdFailedToLoad(error);
        WhenAdFailedToLoaded?.Invoke(this, error);
    }
}
