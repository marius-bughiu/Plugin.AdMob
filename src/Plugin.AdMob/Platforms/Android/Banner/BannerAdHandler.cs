using Android.Gms.Ads;
using Android.Util;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, AdView>
{
    private IAdConsentService? _adConsentService;

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
        _adConsentService = IPlatformApplication.Current!.Services.GetRequiredService<IAdConsentService>();
        _adConsentService.OnConsentInfoUpdated += (_, _) => LoadAd(PlatformView);

        var adUnitId = GetAdUnitId();

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default banner ad unit ID has been configured.");
        }

        var adSize = GetAdSize();

        var adView = new AdView(Context)
        {
            AdSize = adSize,
            AdUnitId = adUnitId
        };

        var listener = new Platforms.Android.AdListener();
        listener.AdLoaded += VirtualView.RaiseOnAdLoaded;
        listener.AdFailedToLoad += (s, e) => VirtualView.RaiseOnAdFailedToLoad(s, new AdError(e.Message));
        listener.AdImpression += VirtualView.RaiseOnAdImpression;
        listener.AdClicked += VirtualView.RaiseOnAdClicked;
        listener.AdSwiped += VirtualView.RaiseOnAdSwiped;
        listener.AdOpened += VirtualView.RaiseOnAdOpened;
        listener.AdClosed += VirtualView.RaiseOnAdClosed;

        adView.AdListener = listener;

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

    private void LoadAd(AdView adView)
    {
        if (CanRequestAds() is false)
        {
            VirtualView.HeightRequest = 0;
            VirtualView.WidthRequest = 0;
            return;
        }

        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);
        MobileAds.RequestConfiguration = configBuilder.Build();

        var requestBuilder = new AdRequest.Builder();
        var adRequest = requestBuilder.Build();

        adView.LoadAd(adRequest);

        VirtualView.HeightRequest = adView.AdSize!.Height;
        VirtualView.WidthRequest = adView.AdSize!.Width;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Banner;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultBannerAdUnitId;
    }

    private Android.Gms.Ads.AdSize GetAdSize()
    {
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
        DisplayMetrics displayMetrics;

        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            displayMetrics = new DisplayMetrics();
            Context.Display!.GetMetrics(displayMetrics);
        }
        else
        {
            displayMetrics = Context.Resources!.DisplayMetrics!;
        }

        return (int)(displayMetrics.WidthPixels / displayMetrics.Density);
    }

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService?.CanRequestAds() ?? false;
    }
}
