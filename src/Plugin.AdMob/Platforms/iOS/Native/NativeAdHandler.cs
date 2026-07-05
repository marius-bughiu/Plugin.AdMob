using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;
using UIKit;

namespace Plugin.AdMob.Handlers;

internal partial class NativeAdHandler : ViewHandler<NativeAdView, Google.MobileAds.NativeAdView>
{
    // Insets applied by the constraints pinning the ad content inside the platform view.
    private const double ContentInset = 8;

    private IAdConsentService? _adConsentService;
    private bool _adContentAttached;

    public static IPropertyMapper<NativeAdView, NativeAdHandler> PropertyMapper
        = new PropertyMapper<NativeAdView, NativeAdHandler>(ViewMapper);
    public NativeAdHandler() : base(PropertyMapper) { }

    protected override void DisconnectHandler(Google.MobileAds.NativeAdView platformView)
    {
        if (_adConsentService is not null)
        {
            _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
        }

        platformView.Dispose();
        _adContentAttached = false;
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
                _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
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
        if (_adContentAttached)
        {
            return;
        }

        this.VirtualView.AdContent.BindingContext = ad;

        var adContentView = this.VirtualView.AdContent.ToPlatform(MauiContext);

        // The platform view is positioned by MAUI via its Frame, so it must keep
        // TranslatesAutoresizingMaskIntoConstraints = true; only the embedded ad
        // content participates in Auto Layout.
        adContentView.TranslatesAutoresizingMaskIntoConstraints = false;

        PlatformView.AddSubview(adContentView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            adContentView.TopAnchor.ConstraintEqualTo(PlatformView.TopAnchor, (nfloat)ContentInset),
            adContentView.LeadingAnchor.ConstraintEqualTo(PlatformView.LeadingAnchor, (nfloat)ContentInset),
            PlatformView.BottomAnchor.ConstraintEqualTo(adContentView.BottomAnchor, (nfloat)ContentInset),
            PlatformView.TrailingAnchor.ConstraintEqualTo(adContentView.TrailingAnchor, (nfloat)ContentInset),
        });

        PlatformView.NativeAd = ((NativeAd)ad).GetPlatformAd();
        VirtualView.BindingContext = ad;

        _adContentAttached = true;
        ((IView)VirtualView).InvalidateMeasure();
    }

    // Google.MobileAds.NativeAdView is a plain UIView whose SizeThatFits reports its
    // current (initially zero) bounds, so MAUI must be given the ad content's size.
    public override Microsoft.Maui.Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        if (!_adContentAttached)
        {
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        var contentSize = ((IView)VirtualView.AdContent).Measure(
            Math.Max(0, widthConstraint - 2 * ContentInset),
            Math.Max(0, heightConstraint - 2 * ContentInset));

        return new Microsoft.Maui.Graphics.Size(
            contentSize.Width + 2 * ContentInset,
            contentSize.Height + 2 * ContentInset);
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

    private void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
    {
        // Consent updates repeatedly (resets, privacy forms); load a single ad once consent allows it.
        if (!AdConfig.DisableConsentCheck && _adConsentService?.CanRequestAds() is not true)
        {
            return;
        }

        // Check if the handler is still connected before loading ad
        // In .NET MAUI 10+, PlatformView throws InvalidOperationException when disconnected
        try
        {
            if (PlatformView is not null)
            {
                _adConsentService!.OnConsentInfoUpdated -= OnConsentInfoUpdated;
                LoadAd();
            }
        }
        catch (InvalidOperationException)
        {
            // Handler has been disconnected, ignore consent update
        }
    }
}
