using Google.Android.Libraries.Ads.Mobile.Sdk.Banner;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd;

namespace Plugin.AdMob.Platforms.Android.Native;

/// <summary>
/// Bridges the next-gen <see cref="INativeAdLoaderCallback" /> to plain C# events.
/// </summary>
internal sealed class NativeAdLoaderCallback : Java.Lang.Object, INativeAdLoaderCallback
{
    public event EventHandler<Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.INativeAd>? AdLoaded;
    public event EventHandler<LoadAdError>? AdFailedToLoad;

    public void OnNativeAdLoaded(Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.INativeAd nativeAd)
        => MainThreadDispatcher.Run(() => AdLoaded?.Invoke(this, nativeAd));

    public void OnAdFailedToLoad(LoadAdError adError)
        => MainThreadDispatcher.Run(() => AdFailedToLoad?.Invoke(this, adError));

    // The plugin only requests the NATIVE ad type, so these never fire.
    public void OnCustomNativeAdLoaded(ICustomNativeAd customNativeAd) { }

    public void OnBannerAdLoaded(IBannerAd bannerAd) { }

    public void OnAdLoadingCompleted() { }
}
