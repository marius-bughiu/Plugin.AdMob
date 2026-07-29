using Android.Runtime;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Plugin.AdMob.Platforms.Android;
using SdkIInterstitialAd = Google.Android.Libraries.Ads.Mobile.Sdk.Interstitial.IInterstitialAd;
using SdkInterstitialAd = Google.Android.Libraries.Ads.Mobile.Sdk.Interstitial.InterstitialAd;

namespace Plugin.AdMob;

internal partial class InterstitialAd
{
    private SdkIInterstitialAd? _ad;

    public void Load()
    {
        AdMobInitializer.RunWhenInitialized(() =>
        {
            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var adRequest = new AdRequest.Builder(AdUnitId).Build();

            var callback = new AdLoadCallback();
            callback.Loaded += (s, ad) => AdLoaded(ad.JavaCast<SdkIInterstitialAd>()!);
            callback.Failed += (s, e) => OnAdFailedToLoad?.Invoke(this, new AdError(e.Message));

            SdkInterstitialAd.Load(adRequest, callback);
        });
    }

    public void Show()
    {
        if (!IsLoaded || _ad is null)
        {
            return;
        }

        var callback = new FullScreenContentCallback();
        callback.AdShowed += (s, e) => OnAdShowed?.Invoke(this, EventArgs.Empty);
        callback.AdFailedToShow += (s, e) => OnAdFailedToShow?.Invoke(this, new AdError(e.Message));
        callback.AdImpression += (s, e) => OnAdImpression?.Invoke(this, EventArgs.Empty);
        callback.AdClicked += (s, e) => OnAdClicked?.Invoke(this, EventArgs.Empty);
        callback.AdDismissed += (s, e) => OnAdDismissed?.Invoke(this, EventArgs.Empty);

        _ad.AdEventCallback = callback;

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        _ad.Show(activity);
    }

    private void AdLoaded(SdkIInterstitialAd interstitialAd)
    {
        _ad = interstitialAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
