using Android.Gms.Ads;
using Android.Util;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, AdView>
{
    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper =
        new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);

    public BannerAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(AdView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override AdView CreatePlatformView()
    {
        var adUnitId = VirtualView.AdUnitId;
        adUnitId ??= AdConfig.DefaultBannerAdUnitId;

        if (AdConfig.UseTestAdUnitIds)
        {
            adUnitId = AdMobTestAdUnits.Banner;
        }

        var adSize = GetAdSize();
        var adView = new AdView(Context) 
        { 
            AdSize = adSize,
            AdUnitId = adUnitId
        };

        var requestBuilder = new AdRequest.Builder();

        var configBuilder = new RequestConfiguration.Builder();

        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);

        MobileAds.RequestConfiguration = configBuilder.Build();

        var adRequest = requestBuilder.Build();

        adView.LoadAd(adRequest);

        VirtualView.HeightRequest = adSize.Height;
        VirtualView.WidthRequest = adSize.Width;

        return adView;
    }   

    private Android.Gms.Ads.AdSize GetAdSize()
    {
        // TODO: Use GetCurrentOrientationAnchoredAdaptiveBannerAdSize instead of SmartBanner

        switch (VirtualView.AdSize)
        {
            case AdSize.Banner: return Android.Gms.Ads.AdSize.Banner;
            case AdSize.LargeBanner: return Android.Gms.Ads.AdSize.LargeBanner;
            case AdSize.MediumRectangle: return Android.Gms.Ads.AdSize.MediumRectangle;
            case AdSize.FullBanner: return Android.Gms.Ads.AdSize.FullBanner;
            case AdSize.Leaderboard: return Android.Gms.Ads.AdSize.Leaderboard;
            case AdSize.Custom: return new Android.Gms.Ads.AdSize(VirtualView.CustomAdWidth, VirtualView.CustomAdHeight);

            case AdSize.SmartBanner:
            default: return Android.Gms.Ads.AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(Context, GetScreenWidth());
        }
    }

    private int GetScreenWidth()
    {
        var displayMetrics = new DisplayMetrics();
        Context.Display.GetMetrics(displayMetrics);

        return (int)(displayMetrics.WidthPixels / displayMetrics.Density);
    }
}
