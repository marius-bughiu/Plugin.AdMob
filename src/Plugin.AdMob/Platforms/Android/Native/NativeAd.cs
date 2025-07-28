using Android.Gms.Ads;
using Plugin.AdMob.Platforms.Android;
using Plugin.AdMob.Platforms.Android.Native;
using AdListener = Plugin.AdMob.Platforms.Android.AdListener;

namespace Plugin.AdMob;

internal partial class NativeAd
{
    private Android.Gms.Ads.NativeAd.NativeAd? _ad;

    public string? Advertiser => _ad?.Advertiser;

    public string? Body => _ad?.Body;

    public string? CallToAction => _ad?.CallToAction;

    public string? Headline => _ad?.Headline;

    //public string? Icon => _ad?.Icon;

    public string? IconUri => _ad?.Icon?.Uri?.ToString();

    //public string? Images => _ad?.Images;

    public string? ImageUri => _ad?.Images?.First()?.Uri?.ToString();

    public string? Price => _ad?.Price;

    public double? StarRating => _ad?.StarRating?.DoubleValue();

    public string? Store => _ad?.Store;

    public void Load()
    {
        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.ApplyGlobalAdConfiguration();
        MobileAds.RequestConfiguration = configBuilder.Build();

        var options = new Android.Gms.Ads.NativeAd.NativeAdOptions.Builder()
            .Build();

        var listener = new AdListener();
        listener.AdFailedToLoad += (s, e) => OnAdFailedToLoad?.Invoke(s, new AdError(e.Message));
        listener.AdImpression += OnAdImpression;
        listener.AdClicked += OnAdClicked;
        listener.AdSwiped += OnAdSwiped;
        listener.AdOpened += OnAdOpened;
        listener.AdClosed += OnAdClosed;

        var nativeAdListener = new NativeAdListener();
        nativeAdListener.AdLoaded += OnAdLoadedInternal;

        AdLoader adLoader = new AdLoader.Builder(Android.App.Application.Context, AdUnitId)
            .WithNativeAdOptions(options)
            .WithAdListener(listener)
            .ForNativeAd(nativeAdListener)
            .Build();

        adLoader.LoadAd(new AdRequest.Builder().Build());
    }

    internal Android.Gms.Ads.NativeAd.NativeAd GetPlatformAd() => _ad!;

    private void OnAdLoadedInternal(object? sender, Android.Gms.Ads.NativeAd.NativeAd nativeAd)
    {
        _ad = nativeAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
