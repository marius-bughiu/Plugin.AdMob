using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, global::Android.Gms.Ads.NativeAd.NativeAdView>
{
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
            IPlatformApplication.Current!.Services
                .GetRequiredService<IAdConsentService>()
                .OnConsentInfoUpdated += (_, _) => LoadAd();
        }
        else
        {
            RegisterEventHandlers(VirtualView._ad);
            ShowAd(VirtualView._ad);
        }
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
        var ad = nativeAdService.CreateAd();

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
}
