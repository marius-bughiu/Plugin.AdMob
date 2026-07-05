using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, global::Android.Gms.Ads.NativeAd.NativeAdView>
{
    private IAdConsentService? _adConsentService;
    private bool _adContentAttached;

    // The ad outlives the handler on disconnect/reconnect, so the subscriptions
    // made in RegisterEventHandlers must be removed to avoid raising events N times.
    private INativeAd? _registeredAd;
    private EventHandler<IAdError>? _onAdFailedToLoad;

    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper =
        new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);

    public NativeAdHandler() : base(PropertyMapper)
    {        
    }

    protected override void ConnectHandler(Android.Gms.Ads.NativeAd.NativeAdView platformView)
    {
        base.ConnectHandler(platformView);

        ArgumentNullException.ThrowIfNull(VirtualView.AdContent, nameof(VirtualView.AdContent));

        if (VirtualView._ad is null)
        {
            _adConsentService = IPlatformApplication.Current!.Services
                .GetRequiredService<IAdConsentService>();

            if (CanRequestAds() is true)
            {
                LoadAd();
            }
            else
            {
                _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
            }
        }
        else
        {
            RegisterEventHandlers(VirtualView._ad);
            ShowAd(VirtualView._ad);
        }
    }

    protected override void DisconnectHandler(global::Android.Gms.Ads.NativeAd.NativeAdView platformView)
    {
        if (_adConsentService is not null)
        {
            _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
        }

        UnregisterEventHandlers();

        _adContentAttached = false;
        base.DisconnectHandler(platformView);
    }

    protected override global::Android.Gms.Ads.NativeAd.NativeAdView CreatePlatformView()
    {
        var platformView = new global::Android.Gms.Ads.NativeAd.NativeAdView(Android.App.Application.Context);
        platformView.CallToActionView = platformView;

        return platformView;
    }

    private void LoadAd()
    {
        var nativeAdService = IPlatformApplication.Current!.Services.GetRequiredService<INativeAdService>();
        var adUnitId = GetAdUnitId();
        var ad = nativeAdService.CreateAd(adUnitId);

        RegisterEventHandlers(ad);
        ad.OnAdLoaded += (s, e) =>
        {
            VirtualView.RaiseOnAdLoaded(s, e);
            ShowAd(ad);
        };

        ad.Load();
    }

    private void ShowAd(INativeAd ad)
    {
        if (_adContentAttached)
        {
            return;
        }

        this.VirtualView.AdContent.BindingContext = ad;

        var adContentView = this.VirtualView.AdContent.ToPlatform(MauiContext!);
        PlatformView.AddView(adContentView);

        PlatformView.SetNativeAd(((NativeAd)ad).GetPlatformAd());
        VirtualView.BindingContext = ad;

        _adContentAttached = true;
    }

    private void RegisterEventHandlers(INativeAd ad)
    {
        UnregisterEventHandlers();

        _onAdFailedToLoad = (s, e) => VirtualView.RaiseOnAdFailedToLoad(s, new AdError(e.Message));

        ad.OnAdFailedToLoad += _onAdFailedToLoad;
        ad.OnAdImpression += VirtualView.RaiseOnAdImpression;
        ad.OnAdClicked += VirtualView.RaiseOnAdClicked;
        ad.OnAdSwiped += VirtualView.RaiseOnAdSwiped;
        ad.OnAdOpened += VirtualView.RaiseOnAdOpened;
        ad.OnAdClosed += VirtualView.RaiseOnAdClosed;

        _registeredAd = ad;
    }

    private void UnregisterEventHandlers()
    {
        if (_registeredAd is null)
        {
            return;
        }

        _registeredAd.OnAdFailedToLoad -= _onAdFailedToLoad;
        _registeredAd.OnAdImpression -= VirtualView.RaiseOnAdImpression;
        _registeredAd.OnAdClicked -= VirtualView.RaiseOnAdClicked;
        _registeredAd.OnAdSwiped -= VirtualView.RaiseOnAdSwiped;
        _registeredAd.OnAdOpened -= VirtualView.RaiseOnAdOpened;
        _registeredAd.OnAdClosed -= VirtualView.RaiseOnAdClosed;

        _registeredAd = null;
        _onAdFailedToLoad = null;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Native;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultNativeAdUnitId;
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
        // Consent updates repeatedly (resets, privacy forms); load a single ad once consent allows it.
        if (CanRequestAds() is false)
        {
            return;
        }

        // Check if the handler is still connected before loading ad
        // In .NET MAUI 10+, PlatformView throws InvalidOperationException when disconnected
        try
        {
            if (PlatformView is not null)
            {
                _adConsentService!.OnConsentInfoUpdated -= OnConsentInfoUpdated;
                LoadAd();
            }
        }
        catch (InvalidOperationException)
        {
            // Handler has been disconnected, ignore consent update
        }
    }
}
