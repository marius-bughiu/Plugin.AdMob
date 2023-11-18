using Android.Gms.Ads;
using Android.Gms.Ads.Hack;
using Android.Gms.Ads.Interstitial;
using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

internal partial class InterstitialAdService : IInterstitialAdService
{
    InterstitialAd _ad;

    public void PrepareAd(string adUnitId)
    {
        adUnitId ??= AdConfig.DefaultInterstitialAdUnitId;

        if (AdConfig.UseTestAdUnitIds)
        {
            adUnitId = AdMobTestAdUnits.Interstitial;
        }

        var requestBuilder = new AdRequest.Builder();

        var configBuilder = new RequestConfiguration.Builder();

        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);

        MobileAds.RequestConfiguration = configBuilder.Build();

        var adRequest = requestBuilder.Build();

        var callback = new MyCallback();
        callback.WhenAdLoaded += ad => _ad = ad;

        InterstitialAd.Load(Android.App.Application.Context, adUnitId, adRequest, callback);
    }

    public void ShowAd()
    {
        if (_ad != null)
        {
            _ad.Show(ActivityStateManager.Default.GetCurrentActivity());

            _ad = null;
            PrepareAd();
        }
        else
        {
            // Log?
        }
    }

    public void PrepareAd()
    {
        PrepareAd(null);
    }
}

public class MyCallback : InterstitialCallback
{
    public delegate void AdLoadedEvent(InterstitialAd ad);

    public event AdLoadedEvent WhenAdLoaded;

    public override void OnAdLoaded(InterstitialAd interstitialAd)
    {
        base.OnAdLoaded(interstitialAd);
        WhenAdLoaded?.Invoke(interstitialAd);
    }
}

class MyAdListener : AdListener
{
    public delegate void AdLoadedEvent();
    public delegate void AdClosedEvent();
    public delegate void AdOpenedEvent();

    public event AdLoadedEvent AdLoaded;
    public event AdClosedEvent AdClosed;
    public event AdOpenedEvent AdOpened;

    public override void OnAdLoaded()
    {
        if (AdLoaded != null) this.AdLoaded();
        base.OnAdLoaded();
    }

    public override void OnAdClosed()
    {
        if (AdClosed != null) this.AdClosed();
        base.OnAdClosed();
    }

    public override void OnAdOpened()
    {
        if (AdOpened != null) this.AdOpened();
        base.OnAdOpened();
    }
}
