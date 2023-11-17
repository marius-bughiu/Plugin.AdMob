using System;
using Android.Runtime;

namespace Android.Gms.Ads.Hack
{
    // https://github.com/xamarin/GooglePlayServicesComponents/issues/425
    public abstract class InterstitialCallback : Android.Gms.Ads.Interstitial.InterstitialAdLoadCallback
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
            InterstitialCallback thisobject = GetObject<InterstitialCallback>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            Android.Gms.Ads.Interstitial.InterstitialAd resultobject = GetObject<Android.Gms.Ads.Interstitial.InterstitialAd>(native_p0, JniHandleOwnership.DoNotTransfer);
            thisobject.OnAdLoaded(resultobject);
        }
    }

    public abstract class RewardedAdLoadCallback : global::Android.Gms.Ads.Rewarded.RewardedAdLoadCallback
    {
        private static Delegate cb_onAdLoaded;

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

        [Register("onAdLoaded", "(Lcom/google/android/gms/ads/rewarded/RewardedAd;)V", "GetOnAdLoadedHandler")]
        public virtual void OnAdLoaded(global::Android.Gms.Ads.Rewarded.RewardedAd rewardedAd)
        {
        }
    }
}