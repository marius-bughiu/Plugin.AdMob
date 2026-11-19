using Foundation;
using Google.MobileAds;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Platforms.iOS;
using Plugin.AdMob.Services;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class BannerAdHandler : ViewHandler<BannerAd, BannerView>
{
    private static readonly TimeSpan ResizeDelay = TimeSpan.FromMilliseconds(500);

    private ConsentGate? _consentGate;
    private ConsentGate.PendingRequest? _pendingAd;
    private EventHandler<BannerViewErrorEventArgs>? _onReceiveAdFailed;
    private Google.MobileAds.AdSize? _requestedAdSize;
    private Google.MobileAds.AdSize? _pendingAdSize;
    private NSTimer? _resizeTimer;
    private int _latestLoad;
    private bool _disconnected;

    public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper
        = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper);
    public BannerAdHandler() : base(PropertyMapper) { }

    public override void PlatformArrange(Rect rect)
    {
        base.PlatformArrange(rect);

        var adSize = GetResizedAdSize();

        if (adSize is { } resizedAdSize && resizedAdSize.Size != _pendingAdSize?.Size)
        {
            _resizeTimer?.Invalidate();
            _resizeTimer = NSTimer.CreateScheduledTimer(ResizeDelay, _ => ReloadIfResized());
        }

        _pendingAdSize = adSize;
    }

    protected override void DisconnectHandler(BannerView platformView)
    {
        _disconnected = true;
        _resizeTimer?.Invalidate();

        _pendingAd?.Dispose();
        _pendingAd = null;

        platformView.AdReceived -= VirtualView.RaiseOnAdLoaded;
        platformView.ReceiveAdFailed -= _onReceiveAdFailed;
        platformView.ImpressionRecorded -= VirtualView.RaiseOnAdImpression;
        platformView.PaidEventHandler = null;
        platformView.ClickRecorded -= VirtualView.RaiseOnAdClicked;
        platformView.WillPresentScreen -= VirtualView.RaiseOnAdOpened;
        platformView.ScreenDismissed -= VirtualView.RaiseOnAdClosed;

        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override BannerView CreatePlatformView()
    {
        var consentGate = new ConsentGate(IPlatformApplication.Current!.Services.GetRequiredService<IAdConsentService>());
        _consentGate = consentGate;
        _disconnected = false;

        var adUnitId = GetAdUnitId();
        var adView = new BannerView()
        {
            AdUnitId = adUnitId
        };

        MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();

        _onReceiveAdFailed = (_, e) => VirtualView.RaiseOnAdFailedToLoad(this, e.Error.ToAdError());

        adView.AdReceived += VirtualView.RaiseOnAdLoaded;
        adView.ReceiveAdFailed += _onReceiveAdFailed;
        adView.ImpressionRecorded += VirtualView.RaiseOnAdImpression;
        adView.PaidEventHandler = value => VirtualView.RaiseOnAdPaid(value.ToRevenueEventArgs(adUnitId));
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
            var pendingAd = consentGate.CreateRequest(() =>
            {
                if (!_disconnected)
                {
                    LoadAd(adView);
                }
            });
            _pendingAd = pendingAd;
            pendingAd.Start();
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

        MobileAdsInitialization.Start();

        // Reset IsLoaded before loading a new ad
        VirtualView.SetValue(BannerAd.IsLoadedProperty, false);

        var adSize = GetAdSize(adView);
        adView.AdSize = adSize;
        _requestedAdSize = adSize;
        var load = ++_latestLoad;

        AdMobInitializer.RunWhenInitialized(() =>
        {
            if (_disconnected || load != _latestLoad)
            {
                return;
            }

            var request = Request.GetDefaultRequest();

            adView.LoadRequest(request);
        }, ex =>
        {
            if (_disconnected || load != _latestLoad)
            {
                return;
            }

            VirtualView.RaiseOnAdFailedToLoad(this, new AdError(ex));
        });

        VirtualView.WidthRequest = adSize.Size.Width;
        VirtualView.HeightRequest = adSize.Size.Height;
    }

    private void ReloadIfResized()
    {
        _pendingAdSize = null;

        if (GetResizedAdSize() is not null)
        {
            LoadAd(PlatformView);
        }
    }

    private Google.MobileAds.AdSize? GetResizedAdSize()
    {
        if (_requestedAdSize is not { } requestedAdSize || VirtualView.AdSize is not (null or AdSize.SmartBanner) || CanRequestAds() is not true)
        {
            return null;
        }

        var adSize = GetAdSize(PlatformView);

        return adSize.Size == requestedAdSize.Size ? null : adSize;
    }

    private Google.MobileAds.AdSize GetAdSize(BannerView adView)
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
            default: return AdSizeCons.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(GetAdaptiveWidth(adView));
        }
    }

    private nfloat GetAdaptiveWidth(BannerView adView)
    {
        var window = adView.Window ?? VirtualView.Window?.Handler?.PlatformView as UIWindow;

        return window is null
            ? UIScreen.MainScreen.Bounds.Width
            : window.SafeAreaInsets.InsetRect(window.Bounds).Width;
    }

    private Google.MobileAds.AdSize GetSize(int width, int height)
        => new Google.MobileAds.AdSize { Size = new CoreGraphics.CGSize(width, height) };

    private bool CanRequestAds() => _consentGate?.CanRequestAds() ?? false;

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Banner;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultBannerAdUnitId;
    }
}
