using Google.MobileAds;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, BannerView>
{
    private IAdConsentService _adConsentService;

    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper 
        = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);
    public BannerAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(BannerView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override BannerView CreatePlatformView()
    {
        _adConsentService = IPlatformApplication.Current.Services.GetService<IAdConsentService>();
        _adConsentService.OnConsentInfoUpdated += (_, _) => LoadAd(PlatformView);

        var adSize = GetAdSize();
        var adView = new BannerView()
        {
            AdSize = adSize,
            RootViewController = Platform.GetCurrentUIViewController()
        };

        if (string.IsNullOrEmpty(adView.AdUnitId) && !string.IsNullOrEmpty(AdConfig.DefaultBannerAdUnitId))
        {
            adView.AdUnitId = AdConfig.DefaultBannerAdUnitId;
        }

        if (AdConfig.UseTestAdUnitIds)
        {
            adView.AdUnitId = AdMobTestAdUnits.Banner;
        }

        MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = [.. AdConfig.TestDevices];        

        adView.AdReceived += VirtualView.RaiseOnAdLoaded;
        adView.ReceiveAdFailed += (s, e) => VirtualView.RaiseOnAdFailedToLoad(this, new AdError(e.Error.DebugDescription));
        adView.ImpressionRecorded += VirtualView.RaiseOnAdImpression;
        adView.ClickRecorded += VirtualView.RaiseOnAdClicked;
        adView.WillPresentScreen += VirtualView.RaiseOnAdOpened;
        adView.ScreenDismissed += VirtualView.RaiseOnAdClosed;

        if (CanRequestAds() is true)
        {
            LoadAd(adView);
        }
        else
        {
            VirtualView.HeightRequest = 0;
            VirtualView.WidthRequest = 0;
        }

        return adView;
    }

    private void LoadAd(BannerView adView)
    {
        if (CanRequestAds() is false)
        {
            VirtualView.HeightRequest = 0;
            VirtualView.WidthRequest = 0;
            return;
        }

        var request = Request.GetDefaultRequest();

        adView.LoadRequest(request);

        var adSize = GetAdSize();
        VirtualView.WidthRequest = adSize.Size.Width;
        VirtualView.HeightRequest = adSize.Size.Height;
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

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService.CanRequestAds();
    }
}
