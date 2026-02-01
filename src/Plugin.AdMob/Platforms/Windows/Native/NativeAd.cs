namespace Plugin.AdMob;

internal partial class NativeAd
{
    public string? Advertiser => null;

    public string? Body => null;

    public string? CallToAction => null;

    public string? Headline => null;

    public string? IconUri => null;

    public string? ImageUri => null;

    public string? Price => null;

    public double? StarRating => null;

    public string? Store => null;

    public object? MediaContent => null;

    public bool HasVideoContent => false;

    public double VideoDuration => 0.0;

    public float VideoAspectRatio => 0.0f;

    public void Load()
    {
        // AdMob is not supported on Windows
        throw new PlatformNotSupportedException("AdMob native ads are not supported on Windows.");
    }

    internal object GetPlatformAd() => throw new PlatformNotSupportedException("AdMob native ads are not supported on Windows.");
}
