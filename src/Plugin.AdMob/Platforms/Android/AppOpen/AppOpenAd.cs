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

    public void Load() => _consentGate.WhenAllowed(LoadAd);

    private void LoadAd()
    {
        MobileAdsInitialization.Start();

        AdMobInitializer.RunWhenInitialized(() =>
        {
            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var adRequest = new AdRequest.Builder(AdUnitId).Build();

            var callback = new AdLoadCallback();
            callback.Loaded += (s, ad) => AdLoaded(ad.JavaCast<SdkIAppOpenAd>()!);
            callback.Failed += (_, error) => OnAdFailedToLoad?.Invoke(this, error.ToAdError());

            SdkAppOpenAd.Load(adRequest, callback);
        }, ex => OnAdFailedToLoad?.Invoke(this, new AdError(ex)));
    }

    public void Show()
    {
        if (_ad is not { } ad || !TryBeginShow())
        {
            return;
        }

        var responseId = ad.ResponseInfo?.ResponseId;
        var callback = new FullScreenContentCallback(AdUnitId);
        callback.AdShowed += (s, e) => OnAdShowed?.Invoke(this, EventArgs.Empty);
        callback.AdFailedToShow += (_, error) => OnAdFailedToShow?.Invoke(this, error.ToAdError(responseId));
        callback.AdImpression += (s, e) => OnAdImpression?.Invoke(this, EventArgs.Empty);
        callback.AdPaid += (_, revenue) => OnAdPaid?.Invoke(this, revenue);
        callback.AdClicked += (s, e) => OnAdClicked?.Invoke(this, EventArgs.Empty);
        callback.AdDismissed += (s, e) => OnAdDismissed?.Invoke(this, EventArgs.Empty);

        ad.AdEventCallback = callback;

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        ad.Show(activity);
    }

    private void AdLoaded(SdkIAppOpenAd appOpenAd)
    {
        _ad = appOpenAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
