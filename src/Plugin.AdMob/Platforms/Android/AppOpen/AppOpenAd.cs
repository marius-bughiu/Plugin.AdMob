using Android.Runtime;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Plugin.AdMob.Platforms.Android;
using SdkAppOpenAd = Google.Android.Libraries.Ads.Mobile.Sdk.AppOpen.AppOpenAd;
using SdkIAppOpenAd = Google.Android.Libraries.Ads.Mobile.Sdk.AppOpen.IAppOpenAd;

namespace Plugin.AdMob;

internal partial class AppOpenAd
{
    private SdkIAppOpenAd? _ad;

    public void Load()
    {
        AdMobInitializer.RunWhenInitialized(() =>
        {
            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var adRequest = new AdRequest.Builder(AdUnitId).Build();

            var callback = new AdLoadCallback();
            callback.Loaded += (s, ad) => AdLoaded(ad.JavaCast<SdkIAppOpenAd>()!);
            callback.Failed += (s, e) => OnAdFailedToLoad?.Invoke(this, new AdError(e.Message));

            SdkAppOpenAd.Load(adRequest, callback);
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

    private void AdLoaded(SdkIAppOpenAd appOpenAd)
    {
        _ad = appOpenAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
