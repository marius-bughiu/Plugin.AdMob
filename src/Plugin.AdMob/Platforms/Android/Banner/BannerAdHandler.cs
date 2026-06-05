using Android.Gms.Ads;
using Android.Util;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Platforms.Android;
using Plugin.AdMob.Services;
using System.Collections.Generic;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, AdView>
{
    // Active banner views are tracked so app lifecycle can pause/resume them
    // without forcing the host app to recreate the MAUI control.
    private static readonly object _activeViewsLock = new();
    private static readonly List<WeakReference<AdView>> _activeViews = [];

    private IAdConsentService? _adConsentService;
    private EventHandler<IConsentInformation?>? _consentInfoUpdatedHandler;

    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper =
        new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);

    public BannerAdHandler() : base(PropertyMapper) { }

    internal static void PauseActiveBanners()
    {
        foreach (var adView in GetActiveViews())
        {
            try
            {
                adView.Pause();
            }
            catch (Exception)
            {
            }
        }
    }

    internal static void ResumeActiveBanners()
    {
        foreach (var adView in GetActiveViews())
        {
            try
            {
                adView.Resume();
            }
            catch (Exception)
            {
            }
        }
    }

    protected override void DisconnectHandler(AdView platformView)
    {
        if (_adConsentService is not null)
        {
            _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
        }

        RemoveActiveView(platformView);
        // Explicitly stop the native AdView before disposal so Android does not
        // keep banner work alive after the handler has been disconnected.
        platformView.AdListener = null;
        platformView.Pause();
        platformView.Destroy();
        platformView.Dispose();
        base.DisconnectHandler(platformView);
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

        var adSize = GetAdSize();

        var adView = new AdView(Context)
        {
            AdSize = adSize,
            AdUnitId = adUnitId
        };

        var listener = new Platforms.Android.AdListener();
        listener.AdLoaded += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdLoaded(s, e));
        listener.AdFailedToLoad += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdFailedToLoad(s, new AdError(e.Message)));
        listener.AdImpression += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdImpression(s, e));
        listener.AdClicked += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdClicked(s, e));
        listener.AdSwiped += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdSwiped(s, e));
        listener.AdOpened += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdOpened(s, e));
        listener.AdClosed += (s, e) => SafeRaise(() => VirtualView.RaiseOnAdClosed(s, e));

        adView.AdListener = listener;

        if (CanRequestAds() is true)
        {
            LoadAd(adView);
        }
        else
        {
            VirtualView.HeightRequest = 0;
            VirtualView.WidthRequest = 0;
        }

        AddActiveView(adView);
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

        // Reset IsLoaded before loading a new ad
        VirtualView.SetValue(BannerAd.IsLoadedProperty, false);

        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.ApplyGlobalAdConfiguration();
        MobileAds.RequestConfiguration = configBuilder.Build();

        var requestBuilder = new AdRequest.Builder();
        var adRequest = requestBuilder.Build();

        adView.LoadAd(adRequest);

        VirtualView.HeightRequest = adView.AdSize!.Height;
        VirtualView.WidthRequest = adView.AdSize!.Width;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Banner;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultBannerAdUnitId;
    }

    private Android.Gms.Ads.AdSize GetAdSize()
    {
        switch (VirtualView.AdSize)
        {
            case AdSize.Banner: return Android.Gms.Ads.AdSize.Banner;
            case AdSize.LargeBanner: return Android.Gms.Ads.AdSize.LargeBanner;
            case AdSize.MediumRectangle: return Android.Gms.Ads.AdSize.MediumRectangle;
            case AdSize.FullBanner: return Android.Gms.Ads.AdSize.FullBanner;
            case AdSize.Leaderboard: return Android.Gms.Ads.AdSize.Leaderboard;
            case AdSize.Custom: return new Android.Gms.Ads.AdSize(VirtualView.CustomAdWidth, VirtualView.CustomAdHeight);

            case AdSize.SmartBanner:
            default: return Android.Gms.Ads.AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(Context, GetScreenWidth());
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

    private static void AddActiveView(AdView adView)
    {
        lock (_activeViewsLock)
        {
            _activeViews.Add(new WeakReference<AdView>(adView));
        }
    }

    private static void RemoveActiveView(AdView adView)
    {
        lock (_activeViewsLock)
        {
            _activeViews.RemoveAll(reference =>
            {
                if (!reference.TryGetTarget(out var target))
                {
                    return true;
                }

                return ReferenceEquals(target, adView);
            });
        }
    }

    private static List<AdView> GetActiveViews()
    {
        List<AdView> activeViews = [];

        lock (_activeViewsLock)
        {
            _activeViews.RemoveAll(reference =>
            {
                if (!reference.TryGetTarget(out var target))
                {
                    return true;
                }

                activeViews.Add(target);
                return false;
            });
        }

        return activeViews;
    }
}
