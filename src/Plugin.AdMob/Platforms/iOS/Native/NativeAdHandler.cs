using Microsoft.Maui.Handlers;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, Google.MobileAds.NativeAdView>
{
    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper
        = new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);
    public NativeAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(Google.MobileAds.NativeAdView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override Google.MobileAds.NativeAdView CreatePlatformView()
    {
        return null;
    }
}
