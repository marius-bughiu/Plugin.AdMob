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

    public bool HasVideoContent => _ad?.MediaContent?.HasVideoContent ?? false;

    public double VideoAspectRatio => _ad?.MediaContent?.AspectRatio ?? 0;

    public TimeSpan VideoDuration => TimeSpan.FromSeconds(_ad?.MediaContent?.Duration ?? 0);

    public TimeSpan VideoCurrentTime => TimeSpan.FromSeconds(_ad?.MediaContent?.CurrentTime ?? 0);

    public bool IsVideoMuted => _ad?.MediaContent?.VideoController?.IsMuted ?? false;

    public bool VideoCustomControlsEnabled => _ad?.MediaContent?.VideoController?.IsCustomControlsEnabled ?? false;

    public bool VideoClickToExpandEnabled => _ad?.MediaContent?.VideoController?.IsClickToExpandEnabled ?? false;

    public void PlayVideo() => _ad?.MediaContent?.VideoController?.Play();

    public void PauseVideo() => _ad?.MediaContent?.VideoController?.Pause();

    public void SetVideoMuted(bool muted) => _ad?.MediaContent?.VideoController?.Mute(muted);

    public void Load()
    {
        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.ApplyGlobalAdConfiguration();
        MobileAds.RequestConfiguration = configBuilder.Build();

        var optionsBuilder = new Android.Gms.Ads.NativeAd.NativeAdOptions.Builder();

        if (VideoOptions is not null)
        {
            optionsBuilder.SetVideoOptions(new Android.Gms.Ads.VideoOptions.Builder()
                .SetStartMuted(VideoOptions.StartMuted)
                .SetCustomControlsRequested(VideoOptions.CustomControlsRequested)
                .SetClickToExpandRequested(VideoOptions.ClickToExpandRequested)
                .Build());
        }

        var options = optionsBuilder.Build();

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

        RegisterVideoLifecycleCallbacks(nativeAd);

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void RegisterVideoLifecycleCallbacks(Android.Gms.Ads.NativeAd.NativeAd nativeAd)
    {
        var videoController = nativeAd.MediaContent?.VideoController;
        if (videoController is null)
        {
            return;
        }

        var callbacks = new VideoLifecycleCallbacks();
        callbacks.WhenVideoStarted += (s, e) => OnVideoStart?.Invoke(this, EventArgs.Empty);
        callbacks.WhenVideoPlayed += (s, e) => OnVideoPlay?.Invoke(this, EventArgs.Empty);
        callbacks.WhenVideoPaused += (s, e) => OnVideoPause?.Invoke(this, EventArgs.Empty);
        callbacks.WhenVideoEnded += (s, e) => OnVideoEnd?.Invoke(this, EventArgs.Empty);
        callbacks.WhenVideoMuted += (s, isMuted) => OnVideoMuted?.Invoke(this, isMuted);

        videoController.SetVideoLifecycleCallbacks(callbacks);
    }
}
