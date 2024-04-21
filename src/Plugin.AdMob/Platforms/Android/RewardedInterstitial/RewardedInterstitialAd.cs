using Android.Gms.Ads;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Platforms.Android.RewardedInterstitial;

namespace Plugin.AdMob;

internal partial class RewardedInterstitialAd
{
    private Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd _ad;

    public void Load()
    {
        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);
        MobileAds.RequestConfiguration = configBuilder.Build();

        var requestBuilder = new AdRequest.Builder();
        var adRequest = requestBuilder.Build();

        var callbacks = new RewardedInterstitialAdCallbacks();
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

        var listener = new RewardedInterstitialAdListener();

        listener.AdShowed += (s, e) => OnAdShowed?.Invoke(s, EventArgs.Empty);
        listener.AdFailedToShow += (s, e) => OnAdFailedToShow?.Invoke(s, new AdError(e.Message));
        listener.AdImpression += (s, e) => OnAdImpression?.Invoke(s, EventArgs.Empty);
        listener.AdClicked += (s, e) => OnAdClicked?.Invoke(s, EventArgs.Empty);
        listener.AdDismissed += (s, e) => OnAdDismissed?.Invoke(s, EventArgs.Empty);

        _ad.FullScreenContentCallback = listener;
        _ad.Show(ActivityStateManager.Default.GetCurrentActivity());
    }

    private void AdLoaded(object sender, Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd interstitialAd)
    {
        _ad = interstitialAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
