using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Configuration;

namespace Plugin.AdMob;

internal partial class NativeAd : NativeAdLoaderDelegate
{
    private Google.MobileAds.NativeAd? _ad;

    public string? Advertiser => _ad?.Advertiser;

    public string? Body => _ad?.Body;

    public string? CallToAction => _ad?.CallToAction;

    // TODO: check why headline property doesn't exist on Google.MobileAds.NativeAd
    public string? Headline => throw new NotImplementedException();

    //public string? Icon => _ad?.Icon;

    public string? IconUri => _ad?.Icon?.ImageUrl?.ToString();

    //public string? Images => _ad?.Images;

    public string? ImageUri => _ad?.Images?.First()?.ImageUrl?.ToString();

    public string? Price => _ad?.Price;

    public double? StarRating => _ad?.StarRating?.DoubleValue;

    public string? Store => _ad?.Store;

    public void Load()
    {
        MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = [.. AdConfig.TestDevices];

        var adLoader = new AdLoader(adUnitID: AdUnitId,
            // The UIViewController parameter is optional.
            rootViewController: null,
            adTypes: [new Foundation.NSString(AdLoaderAdType.Native.ToString())],
            options: null);

        adLoader.Delegate = this;

        var request = Request.GetDefaultRequest();
        adLoader.LoadRequest(request);

        //var listener = new AdListener();
        //listener.AdImpression += OnAdImpression;
        //listener.AdClicked += OnAdClicked;
        //listener.AdSwiped += OnAdSwiped;
        //listener.AdOpened += OnAdOpened;
        //listener.AdClosed += OnAdClosed;
    }

    internal Google.MobileAds.NativeAd GetPlatformAd() => _ad!;

    public override void DidReceiveNativeAd(AdLoader adLoader, Google.MobileAds.NativeAd nativeAd)
    {
        _ad = nativeAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }

    public override void DidFailToReceiveAd(AdLoader adLoader, NSError error)
    {
        OnAdFailedToLoad?.Invoke(this, new AdError(error.Description));
    }
}
