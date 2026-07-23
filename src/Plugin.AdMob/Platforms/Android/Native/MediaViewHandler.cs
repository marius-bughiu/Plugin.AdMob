using Microsoft.Maui.Handlers;

namespace Plugin.AdMob.Handlers;

internal partial class MediaViewHandler : ViewHandler<MediaView, global::Android.Gms.Ads.NativeAd.MediaView>
{
    public static IPropertyMapper<MediaView, MediaViewHandler> PropertyMapper =
        new PropertyMapper<MediaView, MediaViewHandler>(ViewMapper);

    public MediaViewHandler() : base(PropertyMapper)
    {
    }

    protected override global::Android.Gms.Ads.NativeAd.MediaView CreatePlatformView()
        => new(Context);
}
