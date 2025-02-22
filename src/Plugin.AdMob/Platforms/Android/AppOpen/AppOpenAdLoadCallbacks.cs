using Android.Gms.Ads;
using AppOpenAdLoadCallback = Android.Gms.Ads.Workarounds.AppOpenAdLoadCallback;

namespace Plugin.AdMob.Platforms.Android.AppOpen;

internal class AppOpenAdLoadCallbacks : AppOpenAdLoadCallback
{
    public event EventHandler<global::Android.Gms.Ads.AppOpen.AppOpenAd>? WhenAdLoaded;
    public event EventHandler<LoadAdError>? WhenAdFailedToLoaded;

    public override void OnAdLoaded(global::Android.Gms.Ads.AppOpen.AppOpenAd appOpenAd)
    {
        base.OnAdLoaded(appOpenAd);
        WhenAdLoaded?.Invoke(this, appOpenAd);
    }

    public override void OnAdFailedToLoad(LoadAdError error)
    {
        base.OnAdFailedToLoad(error);
        WhenAdFailedToLoaded?.Invoke(this, error);
    }
}
