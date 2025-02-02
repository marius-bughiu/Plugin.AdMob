using Android.Gms.Ads;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Platforms.Android.Interstitial;

namespace Plugin.AdMob;

internal partial class InterstitialAd
{
    private Android.Gms.Ads.Interstitial.InterstitialAd? _ad;

    public void Load()
    {
        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);
        MobileAds.RequestConfiguration = configBuilder.Build();

        var requestBuilder = new AdRequest.Builder();
        var adRequest = requestBuilder.Build();

        var callbacks = new InterstitialAdCallbacks();
        callbacks.WhenAdLoaded += AdLoaded;
        callbacks.WhenAdFailedToLoaded += (s, e) => OnAdFailedToLoad?.Invoke(s, new AdError(e.Message));

        Android.Gms.Ads.Interstitial.InterstitialAd.Load(Android.App.Application.Context, AdUnitId, adRequest, callbacks);
    }

    public void Show()
    {
        if (!IsLoaded)
        {
            return;
        }

        var listener = new InterstitialAdListener();

        listener.AdShowed += (s, e) => OnAdShowed?.Invoke(s, EventArgs.Empty);
        listener.AdFailedToShow += (s, e) => OnAdFailedToShow?.Invoke(s, new AdError(e.Message));
        listener.AdImpression += (s, e) => OnAdImpression?.Invoke(s, EventArgs.Empty);
        listener.AdClicked += (s, e) => OnAdClicked?.Invoke(s, EventArgs.Empty);
        listener.AdDismissed += (s, e) => OnAdDismissed?.Invoke(s, EventArgs.Empty);

        _ad!.FullScreenContentCallback = listener;

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        _ad.Show(activity);
    }

    private void AdLoaded(object? sender, Android.Gms.Ads.Interstitial.InterstitialAd interstitialAd)
    {
        _ad = interstitialAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
