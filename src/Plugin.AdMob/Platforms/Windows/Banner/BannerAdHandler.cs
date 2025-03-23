using Microsoft.Maui.Handlers;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, Microsoft.UI.Xaml.Controls.Border>
{
    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper
        = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);
    public BannerAdHandler() : base(PropertyMapper) { }

    protected override Microsoft.UI.Xaml.Controls.Border CreatePlatformView()
    {
        return new Microsoft.UI.Xaml.Controls.Border() { BorderThickness = new Microsoft.UI.Xaml.Thickness(0) };
    }
}
