using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, global::Android.Gms.Ads.NativeAd.NativeAdView>
{
    private IAdConsentService _adConsentService;

    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper =
        new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);

    public NativeAdHandler() : base(PropertyMapper) 
    {
        _adConsentService = IPlatformApplication.Current!.Services.GetRequiredService<IAdConsentService>();
        _adConsentService.OnConsentInfoUpdated += (_, _) => LoadAd();
    }

    protected override void DisconnectHandler(global::Android.Gms.Ads.NativeAd.NativeAdView platformView)
    {
        platformView.Dispose();
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
        var ad = nativeAdService.CreateAd();

        ad.OnAdLoaded += (s, e) =>
        {
            this.VirtualView.AdContent.BindingContext = ad;

            var adContentView = this.VirtualView.AdContent.ToPlatform(MauiContext);
            PlatformView.AddView(adContentView);

            PlatformView.SetNativeAd((ad as NativeAd).GetPlatformAd());
            VirtualView.BindingContext = ad;
        };

        ad.Load();
    }
}
