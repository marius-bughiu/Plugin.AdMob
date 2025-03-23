using Microsoft.Maui.Handlers;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, Microsoft.UI.Xaml.Controls.Border>
{
    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper
        = new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);
    public NativeAdHandler() : base(PropertyMapper) { }

    protected override Microsoft.UI.Xaml.Controls.Border CreatePlatformView()
    {
        return new Microsoft.UI.Xaml.Controls.Border() { BorderThickness = new Microsoft.UI.Xaml.Thickness(0) };
    }
}
