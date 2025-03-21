namespace Plugin.AdMob.Platforms.Android.Native;

internal class NativeAdListener : Java.Lang.Object, global::Android.Gms.Ads.NativeAd.NativeAd.IOnNativeAdLoadedListener
{
    public event EventHandler<global::Android.Gms.Ads.NativeAd.NativeAd>? AdLoaded;

    public void OnNativeAdLoaded(global::Android.Gms.Ads.NativeAd.NativeAd nativeAd)
    {
        AdLoaded?.Invoke(this, nativeAd);
    }
}
