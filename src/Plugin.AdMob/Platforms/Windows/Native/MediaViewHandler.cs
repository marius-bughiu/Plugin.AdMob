using Microsoft.Maui.Handlers;

namespace Plugin.AdMob.Handlers;

internal partial class MediaViewHandler : ViewHandler<MediaView, Microsoft.UI.Xaml.Controls.Border>
{
    public static IPropertyMapper<MediaView, MediaViewHandler> PropertyMapper =
        new PropertyMapper<MediaView, MediaViewHandler>(ViewMapper);

    public MediaViewHandler() : base(PropertyMapper)
    {
    }

    protected override Microsoft.UI.Xaml.Controls.Border CreatePlatformView() => new();
}
