using System.Diagnostics;
using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Configuration;
using UIKit;

namespace Plugin.AdMob;

public class myDelegate : NativeAdLoaderDelegate
{
    public override void DidReceiveNativeAd(AdLoader adLoader, Google.MobileAds.NativeAd nativeAd)
    {
        // Do something with the native ad
        Debug.WriteLine("DidReceiveNativeAd");
    }

    public override void DidFailToReceiveAd(AdLoader adLoader, NSError error)
    {
        // Handle the error
        Debug.WriteLine("DidFailToReceiveAd");
    }

    public override void DidFinishLoading(AdLoader adLoader)
    {
        // The ad loader has finished loading ads
        Debug.WriteLine("DidFinishLoading");
    }
}

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
        var viewController = UIApplication.SharedApplication.KeyWindow!.RootViewController;
        var nativeAdOptions = new NativeAdImageAdLoaderOptions();

        var adLoader = new AdLoader(adUnitID: "ca-app-pub-3940256099942544/3986624511",
            // The UIViewController parameter is optional.
            rootViewController: viewController,
            adTypes: [new NSString("GADAdLoaderAdTypeNative")],
            //adTypes: [AdLoaderAdType.Native.ToString()],
            //adTypes: ["GADAdLoaderAdTypeNative"],
            options: [nativeAdOptions]);

        adLoader.Delegate = new myDelegate();

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

    // public override void DidReceiveNativeAd(AdLoader adLoader, Google.MobileAds.NativeAd nativeAd)
    // {
    //     _ad = nativeAd;
    //     IsLoaded = true;

    //     OnAdLoaded?.Invoke(this, EventArgs.Empty);
    // }

    // public override void DidFailToReceiveAd(AdLoader adLoader, NSError error)
    // {
    //     OnAdFailedToLoad?.Invoke(this, new AdError(error.Description));
    // }

    // public override void DidFinishLoading(AdLoader adLoader)
    // {
    // }
}
