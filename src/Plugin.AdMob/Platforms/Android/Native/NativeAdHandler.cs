using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;
using SdkMediaView = Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.MediaView;
using SdkNativeAdView = Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.NativeAdView;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, SdkNativeAdView>
{
    private IAdConsentService? _adConsentService;
    private bool _adContentAttached;

    // The ad outlives the handler on disconnect/reconnect, so the subscriptions
    // made in RegisterEventHandlers must be removed to avoid raising events N times.
    private INativeAd? _registeredAd;
    private EventHandler? _onAdLoaded;
    private EventHandler<IAdError>? _onAdFailedToLoad;

    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper =
        new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);

    public NativeAdHandler() : base(PropertyMapper)
    {        
    }

    protected override void ConnectHandler(SdkNativeAdView platformView)
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

    protected override void DisconnectHandler(SdkNativeAdView platformView)
    {
        if (_adConsentService is not null)
        {
            _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
        }

        UnregisterEventHandlers();

        _adContentAttached = false;
        base.DisconnectHandler(platformView);
    }

    protected override SdkNativeAdView CreatePlatformView()
    {
        var platformView = new SdkNativeAdView(Android.App.Application.Context);
        platformView.CallToActionView = platformView;

        return platformView;
    }

    private void LoadAd()
    {
        var nativeAdService = IPlatformApplication.Current!.Services.GetRequiredService<INativeAdService>();
        var adUnitId = GetAdUnitId();
        var ad = nativeAdService.CreateAd(adUnitId);

        RegisterEventHandlers(ad);
        _onAdLoaded = (s, e) =>
        {
            VirtualView.RaiseOnAdLoaded(s, e);
            ShowAd(ad);
        };
        ad.OnAdLoaded += _onAdLoaded;

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

        // The next-gen NativeAdView takes the media view when the ad is registered rather than
        // exposing a settable MediaView property.
        var mediaView = FindMediaView(this.VirtualView.AdContent);

        PlatformView.RegisterNativeAd(((NativeAd)ad).GetPlatformAd(), mediaView);
        VirtualView.BindingContext = ad;

        _adContentAttached = true;
    }

    private static SdkMediaView? FindMediaView(IVisualTreeElement root)
    {
        foreach (var element in root.GetVisualTreeDescendants())
        {
            if (element is MediaView mediaView &&
                mediaView.Handler?.PlatformView is SdkMediaView platformMediaView)
            {
                return platformMediaView;
            }
        }

        return null;
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
        ad.OnVideoStart += VirtualView.RaiseOnVideoStart;
        ad.OnVideoPlay += VirtualView.RaiseOnVideoPlay;
        ad.OnVideoPause += VirtualView.RaiseOnVideoPause;
        ad.OnVideoEnd += VirtualView.RaiseOnVideoEnd;
        ad.OnVideoMuted += VirtualView.RaiseOnVideoMuted;

        _registeredAd = ad;
    }

    private void UnregisterEventHandlers()
    {
        if (_registeredAd is null)
        {
            return;
        }

        if (_onAdLoaded is not null)
        {
            _registeredAd.OnAdLoaded -= _onAdLoaded;
        }

        _registeredAd.OnAdFailedToLoad -= _onAdFailedToLoad;
        _registeredAd.OnAdImpression -= VirtualView.RaiseOnAdImpression;
        _registeredAd.OnAdClicked -= VirtualView.RaiseOnAdClicked;
        _registeredAd.OnAdSwiped -= VirtualView.RaiseOnAdSwiped;
        _registeredAd.OnAdOpened -= VirtualView.RaiseOnAdOpened;
        _registeredAd.OnAdClosed -= VirtualView.RaiseOnAdClosed;
        _registeredAd.OnVideoStart -= VirtualView.RaiseOnVideoStart;
        _registeredAd.OnVideoPlay -= VirtualView.RaiseOnVideoPlay;
        _registeredAd.OnVideoPause -= VirtualView.RaiseOnVideoPause;
        _registeredAd.OnVideoEnd -= VirtualView.RaiseOnVideoEnd;
        _registeredAd.OnVideoMuted -= VirtualView.RaiseOnVideoMuted;

        _registeredAd = null;
        _onAdLoaded = null;
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
