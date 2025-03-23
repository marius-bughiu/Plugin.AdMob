using Microsoft.Maui.Handlers;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, UIView>
{
    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper
        = new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);
    public NativeAdHandler() : base(PropertyMapper) { }

    protected override UIView CreatePlatformView()
    {
        return new UIView();
    }
}
