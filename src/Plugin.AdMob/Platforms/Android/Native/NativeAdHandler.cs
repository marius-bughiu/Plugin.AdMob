using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, global::Android.Gms.Ads.NativeAd.NativeAdView>
{
    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper =
        new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);

    public NativeAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(global::Android.Gms.Ads.NativeAd.NativeAdView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override global::Android.Gms.Ads.NativeAd.NativeAdView CreatePlatformView()
    {
        var platformView = new global::Android.Gms.Ads.NativeAd.NativeAdView(Android.App.Application.Context);
        var adContentView = this.VirtualView.AdContent.ToPlatform(MauiContext);

        var nativeAd = (this.VirtualView.Ad as NativeAd).GetPlatformAd();
        platformView.SetNativeAd(nativeAd);
        platformView.CallToActionView = platformView;

        platformView.AddView(adContentView);

        return platformView;
    }
}
