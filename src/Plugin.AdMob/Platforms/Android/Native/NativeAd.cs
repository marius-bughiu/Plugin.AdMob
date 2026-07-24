using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd;
using Plugin.AdMob.Platforms.Android;
using Plugin.AdMob.Platforms.Android.Native;
using SdkINativeAd = Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.INativeAd;
using SdkNativeAdLoader = Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.NativeAdLoader;
using SdkVideoOptions = Google.Android.Libraries.Ads.Mobile.Sdk.Common.VideoOptions;

namespace Plugin.AdMob;

internal partial class NativeAd
{
    private SdkINativeAd? _ad;

    public string? Advertiser => _ad?.Advertiser;

    public string? Body => _ad?.Body;

    public string? CallToAction => _ad?.CallToAction;

    public string? Headline => _ad?.Headline;

    public string? IconUri => _ad?.Icon?.Uri?.ToString();

    public string? ImageUri => _ad?.Image?.Uri?.ToString();

    public string? Price => _ad?.Price;

    public double? StarRating => _ad?.StarRating?.DoubleValue();

    public string? Store => _ad?.Store;

    public bool HasVideoContent => _ad?.MediaContent?.HasVideoContent ?? false;

    public double VideoAspectRatio => _ad?.MediaContent?.AspectRatio ?? 0;

    public TimeSpan VideoDuration => TimeSpan.FromSeconds(_ad?.MediaContent?.Duration ?? 0);

    public TimeSpan VideoCurrentTime => TimeSpan.FromSeconds(_ad?.MediaContent?.CurrentTime ?? 0);

    public bool IsVideoMuted => _ad?.MediaContent?.VideoController?.IsMuted ?? false;

    public bool VideoCustomControlsEnabled => _ad?.MediaContent?.VideoController?.IsCustomControlsEnabled ?? false;

    // The next-gen video controller no longer reports the click-to-expand state, so we surface what was requested.
    public bool VideoClickToExpandEnabled => VideoOptions?.ClickToExpandRequested ?? false;

    public void PlayVideo() => _ad?.MediaContent?.VideoController?.Play();

    public void PauseVideo() => _ad?.MediaContent?.VideoController?.Pause();

    public void SetVideoMuted(bool muted) => _ad?.MediaContent?.VideoController?.Mute(muted);

    public void Load()
    {
        AdMobInitializer.RunWhenInitialized(() =>
        {
            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var requestBuilder = new NativeAdRequest.Builder(AdUnitId, [NativeAdNativeAdType.Native!]);

            if (VideoOptions is not null)
            {
                requestBuilder.SetVideoOptions(new SdkVideoOptions.Builder()
                    .SetStartMuted(VideoOptions.StartMuted)!
                    .SetCustomControlsRequested(VideoOptions.CustomControlsRequested)!
                    .SetClickToExpandRequested(VideoOptions.ClickToExpandRequested)!
                    .Build());
            }

            var callback = new NativeAdLoaderCallback();
            callback.AdLoaded += (s, ad) => OnAdLoadedInternal(ad);
            callback.AdFailedToLoad += (s, e) => OnAdFailedToLoad?.Invoke(s, new AdError(e.Message));

            SdkNativeAdLoader.Load(requestBuilder.Build(), callback);
        });
    }

    internal SdkINativeAd GetPlatformAd() => _ad!;

    private void OnAdLoadedInternal(SdkINativeAd nativeAd)
    {
        _ad = nativeAd;
        IsLoaded = true;

        RegisterEventCallback(nativeAd);
        RegisterVideoLifecycleCallbacks(nativeAd);

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void RegisterEventCallback(SdkINativeAd nativeAd)
    {
        var callback = new NativeAdEventCallback();
        callback.AdImpression += (s, e) => OnAdImpression?.Invoke(this, EventArgs.Empty);
        callback.AdClicked += (s, e) => OnAdClicked?.Invoke(this, EventArgs.Empty);
        callback.AdSwiped += (s, e) => OnAdSwiped?.Invoke(this, EventArgs.Empty);
        callback.AdOpened += (s, e) => OnAdOpened?.Invoke(this, EventArgs.Empty);
        callback.AdClosed += (s, e) => OnAdClosed?.Invoke(this, EventArgs.Empty);

        nativeAd.AdEventCallback = callback;
    }

    private void RegisterVideoLifecycleCallbacks(SdkINativeAd nativeAd)
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

        videoController.VideoLifecycleCallbacks = callbacks;
    }
}
