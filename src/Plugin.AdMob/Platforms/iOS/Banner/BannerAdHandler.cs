using Google.MobileAds;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, BannerView>
{
    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);

    public BannerAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(BannerView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override BannerView CreatePlatformView()
    {
        var adSize = GetAdSize();
        var adView = new BannerView()
        {
            RootViewController = GetRootViewController()
        };

        if (string.IsNullOrEmpty(adView.AdUnitId) && !string.IsNullOrEmpty(AdConfig.DefaultBannerAdUnitId))
        {
            adView.AdUnitId = AdConfig.DefaultBannerAdUnitId;
        }

        if (AdConfig.UseTestAdUnitIds)
        {
            adView.AdUnitId = AdMobTestAdUnits.Banner;
        }

        MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = AdConfig.TestDevices.ToArray();

        var request = Request.GetDefaultRequest();

        VirtualView.HeightRequest = adSize.Size.Height;
        VirtualView.WidthRequest = adSize.Size.Width;

        adView.AdReceived += VirtualView.RaiseOnAdLoaded;
        adView.ReceiveAdFailed += (s, e) => VirtualView.RaiseOnAdFailedToLoad(this, new AdError(e.Error.DebugDescription));
        adView.ImpressionRecorded += VirtualView.RaiseOnAdImpression;
        adView.ClickRecorded += VirtualView.RaiseOnAdClicked;
        adView.WillPresentScreen += VirtualView.RaiseOnAdOpened;
        adView.ScreenDismissed += VirtualView.RaiseOnAdClosed;

        adView.LoadRequest(request);

        return adView;
    }

    private UIViewController GetRootViewController()
    {
        foreach (UIWindow window in UIApplication.SharedApplication.Windows)
        {
            if (window.RootViewController != null)
            {
                return window.RootViewController;
            }
        }

        return null;
    }

    private Google.MobileAds.AdSize GetAdSize()
    {
        switch (VirtualView.AdSize)
        {
            case AdSize.Banner: return GetSize(320, 50);
            case AdSize.LargeBanner: return GetSize(320, 100);
            case AdSize.MediumRectangle: return GetSize(300, 250);
            case AdSize.FullBanner: return GetSize(468, 60);
            case AdSize.Leaderboard: return GetSize(728, 90);
            case AdSize.Custom: return GetSize(VirtualView.CustomAdWidth, VirtualView.CustomAdHeight);

            case AdSize.SmartBanner:
            default: return AdSizeCons.GetCurrentOrientationAnchoredAdaptiveBannerAdSize((float)UIScreen.MainScreen.Bounds.Width);
        }
    }

    private Google.MobileAds.AdSize GetSize(int width, int height)
        => new Google.MobileAds.AdSize { Size = new CoreGraphics.CGSize(width, height) };
}
