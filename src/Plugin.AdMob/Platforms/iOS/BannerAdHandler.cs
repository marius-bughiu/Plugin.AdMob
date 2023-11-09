using Google.MobileAds;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Helpers;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, BannerView>
{
    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper)
    {

    };

    public BannerAdHandler() : base(PropertyMapper)
    {

    }

    protected override void ConnectHandler(BannerView platformView)
    {
        base.ConnectHandler(platformView);

        // Perform any control setup here
    }

    protected override void DisconnectHandler(BannerView platformView)
    {
        // Perform any native view cleanup here
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override BannerView CreatePlatformView()
    {
        var adView = new BannerView(GetAdSize())
        {
            AdUnitId = VirtualView.AdUnitId,
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

        var request = Request.GetDefaultRequest();

        // TODO: test devices are missing
        //if (AdConfig.TestDevices.Any())
        //{
        //    request.TestDevices = AdConfig.TestDevices.ToArray();
        //}

        VirtualView.HeightRequest = GetSmartBannerHeightDp();
        adView.LoadRequest(request);

        return adView;
    }

    private int GetSmartBannerHeightDp()
    {
        var screenHeightDp = (float)UIScreen.MainScreen.Bounds.Height;
        return BannerSizeHelper.GetSmartBannerHeight(screenHeightDp);
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
            case AdSize.Banner: return AdSizeCons.Banner;
            case AdSize.LargeBanner: return AdSizeCons.LargeBanner;
            case AdSize.MediumRectangle: return AdSizeCons.MediumRectangle;
            case AdSize.FullBanner: return AdSizeCons.FullBanner;
            case AdSize.Leaderboard: return AdSizeCons.Leaderboard;
            case AdSize.Custom: return new Google.MobileAds.AdSize { Size = new CoreGraphics.CGSize(VirtualView.CustomAdWidth, VirtualView.CustomAdHeight) };
            default: return AdSizeCons.GetCurrentOrientationAnchoredAdaptiveBannerAdSize((float)UIScreen.MainScreen.Bounds.Width);
        }
    }
}
