using Microsoft.Maui.Handlers;
using SdkMediaView = Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd.MediaView;

namespace Plugin.AdMob.Handlers;

internal partial class MediaViewHandler : ViewHandler<MediaView, SdkMediaView>
{
    public static IPropertyMapper<MediaView, MediaViewHandler> PropertyMapper =
        new PropertyMapper<MediaView, MediaViewHandler>(ViewMapper);

    public MediaViewHandler() : base(PropertyMapper)
    {
    }

    protected override SdkMediaView CreatePlatformView()
        => new(Context);
}
