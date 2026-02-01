using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, global::Android.Gms.Ads.NativeAd.NativeAdView>
{
    private IAdConsentService? _adConsentService;

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
            _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
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
        this.VirtualView.AdContent.BindingContext = ad;

        var adContentView = this.VirtualView.AdContent.ToPlatform(MauiContext!);
        PlatformView.AddView(adContentView);

        PlatformView.SetNativeAd(((NativeAd)ad).GetPlatformAd());
        VirtualView.BindingContext = ad;
    }

    private void RegisterEventHandlers(INativeAd ad)
    {
        ad.OnAdFailedToLoad += (s, e) => VirtualView.RaiseOnAdFailedToLoad(s, new AdError(e.Message));
        ad.OnAdImpression += VirtualView.RaiseOnAdImpression;
        ad.OnAdClicked += VirtualView.RaiseOnAdClicked;
        ad.OnAdSwiped += VirtualView.RaiseOnAdSwiped;
        ad.OnAdOpened += VirtualView.RaiseOnAdOpened;
        ad.OnAdClosed += VirtualView.RaiseOnAdClosed;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Banner;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultBannerAdUnitId;
    }

    private void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
    {
        // Check if the handler is still connected before loading ad
        // In .NET MAUI 10+, PlatformView throws InvalidOperationException when disconnected
        try
        {
            if (PlatformView is not null)
            {
                LoadAd();
            }
        }
        catch (InvalidOperationException)
        {
            // Handler has been disconnected, ignore consent update
        }
    }
}
