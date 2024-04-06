using Android.Runtime;

namespace Android.Gms.Ads.Workarounds;

/// <summary>
/// Temporary workaround until the bindings libraries are updated.
/// Source: https://github.com/xamarin/GooglePlayServicesComponents/issues/425
/// </summary>
public abstract class InterstitialAdLoadCallback : Android.Gms.Ads.Interstitial.InterstitialAdLoadCallback
{
    [Register("onAdLoaded", "(Lcom/google/android/gms/ads/interstitial/InterstitialAd;)V", "GetOnAdLoadedHandler")]
    public virtual void OnAdLoaded(Android.Gms.Ads.Interstitial.InterstitialAd interstitialAd)
    {

    }

    private static Delegate cb_onAdLoaded;

    private static Delegate GetOnAdLoadedHandler()
    {
        if (cb_onAdLoaded is null)
            cb_onAdLoaded = JNINativeWrapper.CreateDelegate((Action<IntPtr, IntPtr, IntPtr>)n_onAdLoaded);
        return cb_onAdLoaded;
    }

    private static void n_onAdLoaded(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
    {
        InterstitialAdLoadCallback thisobject = GetObject<InterstitialAdLoadCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
        Android.Gms.Ads.Interstitial.InterstitialAd resultobject = GetObject<Android.Gms.Ads.Interstitial.InterstitialAd>(native_p0, JniHandleOwnership.DoNotTransfer);
        thisobject.OnAdLoaded(resultobject);
    }
}

/// <summary>
/// Temporary workaround until the bindings libraries are updated.
/// Source: https://github.com/xamarin/GooglePlayServicesComponents/issues/425
/// </summary>
public abstract class RewardedAdLoadCallback : global::Android.Gms.Ads.Rewarded.RewardedAdLoadCallback
{
    private static Delegate cb_onAdLoaded;

    [Register("onAdLoaded", "(Lcom/google/android/gms/ads/rewarded/RewardedAd;)V", "GetOnAdLoadedHandler")]
    public virtual void OnAdLoaded(global::Android.Gms.Ads.Rewarded.RewardedAd rewardedAd)
    {

    }

    private static Delegate GetOnAdLoadedHandler()
    {
        if (cb_onAdLoaded is null)
        {
            cb_onAdLoaded = JNINativeWrapper.CreateDelegate((Action<IntPtr, IntPtr, IntPtr>)n_onAdLoaded);
        }

        return cb_onAdLoaded;
    }

    private static void n_onAdLoaded(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
    {
        RewardedAdLoadCallback thisobject = GetObject<RewardedAdLoadCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
        global::Android.Gms.Ads.Rewarded.RewardedAd resultobject = GetObject<global::Android.Gms.Ads.Rewarded.RewardedAd>(native_p0, JniHandleOwnership.DoNotTransfer);
        thisobject.OnAdLoaded(resultobject);
    }
}