using Android.Runtime;

namespace Android.Gms.Ads.Workarounds;

/// <summary>
/// Temporary workaround until the bindings libraries are updated.
/// Source: https://github.com/xamarin/GooglePlayServicesComponents/issues/425
/// </summary>
public abstract class InterstitialAdLoadCallback : Interstitial.InterstitialAdLoadCallback
{
    [Register("onAdLoaded", "(Lcom/google/android/gms/ads/interstitial/InterstitialAd;)V", "GetOnAdLoadedHandler")]
    public virtual void OnAdLoaded(Android.Gms.Ads.Interstitial.InterstitialAd interstitialAd)
    {

    }

    private static Delegate? cb_onAdLoaded;

#pragma warning disable IDE0051 // Remove unused private members
    private static Delegate GetOnAdLoadedHandler()
#pragma warning restore IDE0051 // Remove unused private members
    {
        if (cb_onAdLoaded is null)
            cb_onAdLoaded = JNINativeWrapper.CreateDelegate(n_onAdLoaded);
        return cb_onAdLoaded;
    }

    private static void n_onAdLoaded(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
    {
        InterstitialAdLoadCallback @this = GetObject<InterstitialAdLoadCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
        Interstitial.InterstitialAd result = GetObject<Interstitial.InterstitialAd>(native_p0, JniHandleOwnership.DoNotTransfer)!;
        @this.OnAdLoaded(result);
    }
}

/// <summary>
/// Temporary workaround until the bindings libraries are updated.
/// Source: https://github.com/xamarin/GooglePlayServicesComponents/issues/425
/// </summary>
public abstract class RewardedAdLoadCallback : Rewarded.RewardedAdLoadCallback
{
    private static Delegate? cb_onAdLoaded;

    [Register("onAdLoaded", "(Lcom/google/android/gms/ads/rewarded/RewardedAd;)V", "GetOnAdLoadedHandler")]
    public virtual void OnAdLoaded(Rewarded.RewardedAd rewardedAd)
    {

    }

#pragma warning disable IDE0051 // Remove unused private members
    private static Delegate GetOnAdLoadedHandler()
#pragma warning restore IDE0051 // Remove unused private members
    {
        if (cb_onAdLoaded is null)
        {
            cb_onAdLoaded = JNINativeWrapper.CreateDelegate(n_onAdLoaded);
        }

        return cb_onAdLoaded;
    }

    private static void n_onAdLoaded(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
    {
        RewardedAdLoadCallback @this = GetObject<RewardedAdLoadCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
        Rewarded.RewardedAd result = GetObject<Rewarded.RewardedAd>(native_p0, JniHandleOwnership.DoNotTransfer)!;
        @this.OnAdLoaded(result);
    }
}

/// <summary>
/// Temporary workaround until the bindings libraries are updated
/// Source: https://github.com/xamarin/GooglePlayServicesComponents/issues/425
/// </summary>
public abstract class RewardedInterstitialAdLoadCallback : RewardedInterstitial.RewardedInterstitialAdLoadCallback
{
    [Register("onAdLoaded", "(Lcom/google/android/gms/ads/rewardedinterstitial/RewardedInterstitialAd;)V", "GetOnAdLoadedHandler")]
    public virtual void OnAdLoaded(RewardedInterstitial.RewardedInterstitialAd rewardedInterstitialAd)
    {
    }
    
    private static Delegate? cb_onAdLoaded;

#pragma warning disable IDE0051 // Remove unused private members
    private static Delegate GetOnAdLoadedHandler()
#pragma warning restore IDE0051 // Remove unused private members
    {
        if (cb_onAdLoaded is null)
            cb_onAdLoaded = JNINativeWrapper.CreateDelegate(n_onAdLoaded);
        return cb_onAdLoaded;
    }
    private static void n_onAdLoaded(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
    {
        var @this = GetObject<RewardedInterstitialAdLoadCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
        var result = GetObject<global::Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd>(native_p0, JniHandleOwnership.DoNotTransfer)!;
        @this.OnAdLoaded(result);
    }
}