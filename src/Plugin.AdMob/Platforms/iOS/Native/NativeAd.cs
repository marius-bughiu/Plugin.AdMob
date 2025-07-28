using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Platforms.iOS;

namespace Plugin.AdMob;

internal partial class NativeAd : NativeAdLoaderDelegate
{
    private Google.MobileAds.NativeAd? _ad;

    public string? Advertiser => _ad?.Advertiser;

    public string? Body => _ad?.Body;

    public string? CallToAction => _ad?.CallToAction;

    public string? Headline => _ad?.Headline;

    //public string? Icon => _ad?.Icon;

    public string? IconUri => _ad?.Icon?.ImageUrl?.ToString();

    //public string? Images => _ad?.Images;

    public string? ImageUri => _ad?.Images?.First()?.ImageUrl?.ToString();

    public string? Price => _ad?.Price;

    public double? StarRating => _ad?.StarRating?.DoubleValue;

    public string? Store => _ad?.Store;

    public void Load()
    {
        MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
        var adLoader = new AdLoader(adUnitID: AdUnitId,
            // The UIViewController parameter is optional.
            rootViewController: null,
            adTypes: [AdLoadAdTypeConstants.Native],
            options: []);

        adLoader.Delegate = this;

        var request = Request.GetDefaultRequest();
        adLoader.LoadRequest(request);
    }

    internal Google.MobileAds.NativeAd GetPlatformAd() => _ad!;

    public override void DidReceiveNativeAd(AdLoader adLoader, Google.MobileAds.NativeAd nativeAd)
    {
        _ad = nativeAd;
        IsLoaded = true;

        _ad.ImpressionRecorded += (s, e) => OnAdImpression?.Invoke(this, EventArgs.Empty);
        _ad.ClickRecorded += (s, e) => OnAdClicked?.Invoke(this, EventArgs.Empty);
        _ad.WillPresentScreen += (s, e) => OnAdOpened?.Invoke(this, EventArgs.Empty);
        _ad.ScreenDismissed += (s, e) => OnAdClosed?.Invoke(this, EventArgs.Empty);

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }

    public override void DidFailToReceiveAd(AdLoader adLoader, NSError error)
    {
        OnAdFailedToLoad?.Invoke(this, new AdError(error.Description));
    }
}
