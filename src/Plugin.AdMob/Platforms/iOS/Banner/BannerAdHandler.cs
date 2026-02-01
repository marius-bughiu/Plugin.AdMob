using Google.MobileAds;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Platforms.iOS;
using Plugin.AdMob.Services;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, BannerView>
{
    private IAdConsentService? _adConsentService;

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
        _adConsentService = IPlatformApplication.Current!.Services.GetRequiredService<IAdConsentService>();
        _adConsentService.OnConsentInfoUpdated += (_, _) => LoadAd(PlatformView);

        var adSize = GetAdSize();
        var adView = new BannerView()
        {
            AdSize = adSize,
            AdUnitId = GetAdUnitId()
        };

        // Apply global ad configuration if SDK is initialized
        var sharedInstance = MobileAds.SharedInstance;
        if (sharedInstance is not null)
        {
            sharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("WARNING: MobileAds.SharedInstance is null in BannerAdHandler. Ad configuration may not be applied.");
        }

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

        return _adConsentService?.CanRequestAds() ?? false;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Banner;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultBannerAdUnitId;
    }
}
