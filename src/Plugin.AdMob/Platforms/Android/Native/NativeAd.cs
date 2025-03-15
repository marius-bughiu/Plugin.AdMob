using Android.Gms.Ads;
using Plugin.AdMob.Platforms.Android.Native;
using AdListener = Plugin.AdMob.Platforms.Android.AdListener;

namespace Plugin.AdMob;

internal partial class NativeAd
{
    private Android.Gms.Ads.NativeAd.NativeAd? _ad;

    public string Image => _ad.Images.First().Uri.ToString();

    public void Load()
    {
        var options = new Android.Gms.Ads.NativeAd.NativeAdOptions.Builder()
            .Build();

        var listener = new AdListener();
        //listener.AdLoaded += (s, _) => OnAdLoaded?.Invoke(s, EventArgs.Empty);
        listener.AdFailedToLoad += (s, e) => OnAdFailedToLoad?.Invoke(s, new AdError(e.Message));

        var nativeAdListener = new NativeAdListener();
        nativeAdListener.AdLoaded += AdLoaded;

        AdLoader adLoader = new AdLoader.Builder(Android.App.Application.Context, AdUnitId)
            .WithNativeAdOptions(options)
            .WithAdListener(listener)
            .ForNativeAd(nativeAdListener)
            .Build();

        adLoader.LoadAd(new AdRequest.Builder().Build());
    }

    internal Android.Gms.Ads.NativeAd.NativeAd GetPlatformAd() => _ad!;

    private void AdLoaded(object? sender, Android.Gms.Ads.NativeAd.NativeAd nativeAd)
    {
        _ad = nativeAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
