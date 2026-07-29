using Android.Runtime;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Plugin.AdMob.Platforms.Android;
using SdkIRewardedInterstitialAd = Google.Android.Libraries.Ads.Mobile.Sdk.RewardedInterstitial.IRewardedInterstitialAd;
using SdkRewardedInterstitialAd = Google.Android.Libraries.Ads.Mobile.Sdk.RewardedInterstitial.RewardedInterstitialAd;

namespace Plugin.AdMob;

internal partial class RewardedInterstitialAd
{
    private SdkIRewardedInterstitialAd? _ad;

    public void Load()
    {
        AdMobInitializer.RunWhenInitialized(() =>
        {
            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var adRequest = new AdRequest.Builder(AdUnitId).Build();

            var callback = new AdLoadCallback();
            callback.Loaded += (s, ad) => AdLoaded(ad.JavaCast<SdkIRewardedInterstitialAd>()!);
            callback.Failed += (s, e) => OnAdFailedToLoad?.Invoke(this, new AdError(e.Message));

            SdkRewardedInterstitialAd.Load(adRequest, callback);
        });
    }

    public void Show()
    {
        if (!IsLoaded || _ad is null)
        {
            return;
        }

        var callback = new FullScreenContentCallback();
        callback.AdShowed += (s, _) => OnAdShowed?.Invoke(this, EventArgs.Empty);
        callback.AdFailedToShow += (s, e) => OnAdFailedToShow?.Invoke(this, new AdError(e.Message));
        callback.AdImpression += (s, _) => OnAdImpression?.Invoke(this, EventArgs.Empty);
        callback.AdClicked += (s, _) => OnAdClicked?.Invoke(this, EventArgs.Empty);
        callback.AdDismissed += (s, _) => OnAdDismissed?.Invoke(this, EventArgs.Empty);

        _ad.AdEventCallback = callback;

        var rewardListener = new UserEarnedRewardListener();
        rewardListener.UserEarnedReward += (s, reward) => OnUserEarnedReward?.Invoke(this, new RewardItem(reward.Amount, reward.Type));

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        _ad.Show(activity, rewardListener);
    }

    private void AdLoaded(SdkIRewardedInterstitialAd rewardedInterstitialAd)
    {
        _ad = rewardedInterstitialAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
