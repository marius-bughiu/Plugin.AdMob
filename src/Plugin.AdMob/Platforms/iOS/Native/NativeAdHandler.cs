using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, Google.MobileAds.NativeAdView>
{
    private IAdConsentService? _adConsentService;
    private EventHandler? _consentInfoUpdatedHandler;

    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper
        = new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);
    public NativeAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(Google.MobileAds.NativeAdView platformView)
    {
        // Unsubscribe from consent service events to avoid accessing disposed objects
        if (_adConsentService is not null && _consentInfoUpdatedHandler is not null)
        {
            _adConsentService.OnConsentInfoUpdated -= _consentInfoUpdatedHandler;
            _consentInfoUpdatedHandler = null;
        }

        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }

    protected override void ConnectHandler(Google.MobileAds.NativeAdView platformView)
    {
        base.ConnectHandler(platformView);

        ArgumentNullException.ThrowIfNull(VirtualView.AdContent, nameof(VirtualView.AdContent));

        if (VirtualView._ad is null)
        {
            _adConsentService = IPlatformApplication.Current!.Services.GetRequiredService<IAdConsentService>();
            if (CanRequestAds())
            {
                LoadAd();
            }
            else
            {
                _consentInfoUpdatedHandler = (_, _) => LoadAd();
                _adConsentService.OnConsentInfoUpdated += _consentInfoUpdatedHandler;
            }

            bool CanRequestAds()
            {
                if (AdConfig.DisableConsentCheck)
                {
                    return true;
                }

                return _adConsentService.CanRequestAds();
            }
        }
        else
        {
            RegisterEventHandlers(VirtualView._ad);
            ShowAd(VirtualView._ad);
        }
    }

    protected override Google.MobileAds.NativeAdView CreatePlatformView()
    {
        var platformView = new Google.MobileAds.NativeAdView();
        platformView.CallToActionView = platformView;

        return platformView;
    }

    private void LoadAd()
    {
        var nativeAdService = IPlatformApplication.Current!.Services.GetRequiredService<INativeAdService>();
        var adUnitId = GetAdUnitId();
        var ad = nativeAdService.CreateAd(adUnitId);

        RegisterEventHandlers(ad);
        ad.OnAdLoaded += (s, e) =>
        {
            VirtualView.RaiseOnAdLoaded(s, e);
            ShowAd(ad);
        };

        ad.Load();
    }

    private void ShowAd(INativeAd ad)
    {
        this.VirtualView.AdContent.BindingContext = ad;

        var adContentView = this.VirtualView.AdContent.ToPlatform(MauiContext);
        PlatformView.TranslatesAutoresizingMaskIntoConstraints = false;

        PlatformView.AddSubview(adContentView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            // Pin the label's top/left to container
            adContentView.TopAnchor.ConstraintEqualTo(PlatformView.TopAnchor, 8),
            adContentView.LeadingAnchor.ConstraintEqualTo(PlatformView.LeadingAnchor, 8),
            
            // Make the container expand by pinning container’s bottom/trailing 
            // to label’s bottom/trailing
            PlatformView.BottomAnchor.ConstraintEqualTo(adContentView.BottomAnchor, 8),
            PlatformView.TrailingAnchor.ConstraintEqualTo(adContentView.TrailingAnchor, 8),
        });

        PlatformView.NativeAd = ((NativeAd)ad).GetPlatformAd();
        VirtualView.BindingContext = ad;
    }

    private void RegisterEventHandlers(INativeAd ad)
    {
        ad.OnAdFailedToLoad += (s, e) => VirtualView.RaiseOnAdFailedToLoad(s, new AdError(e.Message));
        ad.OnAdImpression += VirtualView.RaiseOnAdImpression;
        ad.OnAdClicked += VirtualView.RaiseOnAdClicked;
        ad.OnAdSwiped += VirtualView.RaiseOnAdSwiped;
        ad.OnAdOpened += VirtualView.RaiseOnAdOpened;
        ad.OnAdClosed += VirtualView.RaiseOnAdClosed;
    }

    private string? GetAdUnitId()
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Native;
        }

        return VirtualView.AdUnitId ?? AdConfig.DefaultNativeAdUnitId;
    }
}
