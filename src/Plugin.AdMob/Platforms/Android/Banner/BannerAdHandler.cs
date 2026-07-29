using Android.Runtime;
using Android.Util;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Banner;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Platforms.Android;
using Plugin.AdMob.Platforms.Android.Banner;
using Plugin.AdMob.Services;
using AdView = Google.Android.Libraries.Ads.Mobile.Sdk.Banner.AdView;
using SdkAdSize = Google.Android.Libraries.Ads.Mobile.Sdk.Banner.AdSize;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, AdView>
{
    private IAdConsentService? _adConsentService;

    // Loading an ad is deferred until the SDK finishes initializing, so the handler can be disconnected
    // (and the AdView destroyed) while a load is still queued. The queued work checks this before running.
    private bool _disconnected;

    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper =
        new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);

    public BannerAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(AdView platformView)
    {
        _disconnected = true;

        if (_adConsentService is not null)
        {
            _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
        }

        try
        {
            // Explicitly stop the native AdView before disposal so the SDK does not keep banner work alive
            // after the handler has been disconnected.
            platformView.Destroy();
            platformView.Dispose();
        }
        catch (Exception)
        {
        }
        finally
        {
            base.DisconnectHandler(platformView);
        }
    }

    protected override AdView CreatePlatformView()
    {
        _adConsentService = IPlatformApplication.Current!.Services.GetRequiredService<IAdConsentService>();
        _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;

        var adUnitId = GetAdUnitId();

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default banner ad unit ID has been configured.");
        }

        var adView = new AdView(Context);

        if (CanRequestAds() is true)
        {
            LoadAd(adView);
        }
        else
        {
            VirtualView.HeightRequest = 0;
            VirtualView.WidthRequest = 0;
        }

        return adView;
    }

    private void LoadAd(AdView adView)
    {
        if (CanRequestAds() is false)
        {
            VirtualView.HeightRequest = 0;
            VirtualView.WidthRequest = 0;
            return;
        }

        var adUnitId = GetAdUnitId()!;
        var adSize = GetAdSize();

        // Reset IsLoaded before loading a new ad
        VirtualView.SetValue(BannerAd.IsLoadedProperty, false);

        AdMobInitializer.RunWhenInitialized(() =>
        {
            // The handler can be disconnected while the SDK is still initializing, in which case the
            // AdView captured above has already been destroyed and disposed.
            if (_disconnected)
            {
                return;
            }

            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var adRequest = new BannerAdRequest.Builder(adUnitId, adSize).Build();

            var callback = new AdLoadCallback();
            callback.Loaded += (s, ad) =>
            {
                var bannerAd = ad.JavaCast<IBannerAd>();
                if (bannerAd is not null)
                {
                    var eventCallback = new BannerAdEventCallback();
                    eventCallback.AdImpression += (s2, e) => SafeRaise(() => VirtualView.RaiseOnAdImpression(s2, e));
                    eventCallback.AdClicked += (s2, e) => SafeRaise(() => VirtualView.RaiseOnAdClicked(s2, e));
                    eventCallback.AdOpened += (s2, e) => SafeRaise(() => VirtualView.RaiseOnAdOpened(s2, e));
                    eventCallback.AdClosed += (s2, e) => SafeRaise(() => VirtualView.RaiseOnAdClosed(s2, e));
                    bannerAd.AdEventCallback = eventCallback;
                }

                SafeRaise(() => VirtualView.RaiseOnAdLoaded(s, EventArgs.Empty));
            };
            callback.Failed += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdFailedToLoad(s, new AdError(e.Message)));

            try
            {
                adView.LoadAd(adRequest, callback);
            }
            catch (ObjectDisposedException)
            {
                // The AdView was disposed after the check above; nothing left to load into.
            }
        });

        VirtualView.HeightRequest = adSize.Height;
        VirtualView.WidthRequest = adSize.Width;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Banner;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultBannerAdUnitId;
    }

    private SdkAdSize GetAdSize()
    {
        switch (VirtualView.AdSize)
        {
            case AdSize.Banner: return SdkAdSize.Banner!;
            case AdSize.LargeBanner: return SdkAdSize.LargeBanner!;
            case AdSize.MediumRectangle: return SdkAdSize.MediumRectangle!;
            case AdSize.FullBanner: return SdkAdSize.FullBanner!;
            case AdSize.Leaderboard: return SdkAdSize.Leaderboard!;
            case AdSize.Custom: return new SdkAdSize(VirtualView.CustomAdWidth, VirtualView.CustomAdHeight);

            case AdSize.SmartBanner:
            default: return SdkAdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(Context, GetScreenWidth());
        }
    }

    private int GetScreenWidth()
    {
        DisplayMetrics displayMetrics;

        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            displayMetrics = new DisplayMetrics();
            Context.Display!.GetMetrics(displayMetrics);
        }
        else
        {
            displayMetrics = Context.Resources!.DisplayMetrics!;
        }

        return (int)(displayMetrics.WidthPixels / displayMetrics.Density);
    }

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService?.CanRequestAds() ?? false;
    }

    private void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
    {
        // Check if the handler is still connected before accessing PlatformView
        // In .NET MAUI 10+, PlatformView throws InvalidOperationException when disconnected
        try
        {
            var adView = PlatformView;
            if (adView is not null)
            {
                LoadAd(adView);
            }
        }
        catch (InvalidOperationException)
        {
            // Handler has been disconnected, ignore consent update
        }
    }

    private void SafeRaise(Action action)
    {
        try
        {
            action();
        }
        catch (InvalidOperationException)
        {
            // Handler has been disconnected, ignore ad event.
            // This prevents: System.InvalidOperationException: VirtualView cannot be null here
        }
    }
}
