using Microsoft.Maui.Handlers;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, UIView>
{
    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper
        = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);
    public BannerAdHandler() : base(PropertyMapper) { }

    protected override UIView CreatePlatformView()
    {
        return new UIView();
    }
}
