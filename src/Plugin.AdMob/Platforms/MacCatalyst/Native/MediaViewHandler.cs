using Microsoft.Maui.Handlers;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class MediaViewHandler : ViewHandler<MediaView, UIView>
{
    public static IPropertyMapper<MediaView, MediaViewHandler> PropertyMapper =
        new PropertyMapper<MediaView, MediaViewHandler>(ViewMapper);

    public MediaViewHandler() : base(PropertyMapper)
    {
    }

    protected override UIView CreatePlatformView() => new();
}
