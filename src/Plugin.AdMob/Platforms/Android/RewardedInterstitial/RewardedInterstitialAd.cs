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

    public void Load() => _consentGate.WhenAllowed(LoadAd);

    private void LoadAd()
    {
        AdMobInitializer.RunWhenInitialized(() =>
        {
            var configBuilder = new RequestConfiguration.Builder();
            configBuilder.ApplyGlobalAdConfiguration();
            MobileAds.RequestConfiguration = configBuilder.Build();

            var adRequest = new AdRequest.Builder(AdUnitId).Build();

            var callback = new AdLoadCallback();
            callback.Loaded += (s, ad) => AdLoaded(ad.JavaCast<SdkIRewardedInterstitialAd>()!);
            callback.Failed += (_, error) => OnAdFailedToLoad?.Invoke(this, error.ToAdError());

            SdkRewardedInterstitialAd.Load(adRequest, callback);
        }, ex => OnAdFailedToLoad?.Invoke(this, new AdError(ex)));
    }

    public void Show()
    {
        if (_ad is not { } ad || !TryBeginShow())
        {
            return;
        }

        var callback = new FullScreenContentCallback(AdUnitId);
        callback.AdShowed += (s, _) => OnAdShowed?.Invoke(this, EventArgs.Empty);
        callback.AdFailedToShow += (_, error) => OnAdFailedToShow?.Invoke(this, error.ToAdError(ad.ResponseInfo?.ResponseId));
        callback.AdImpression += (s, _) => OnAdImpression?.Invoke(this, EventArgs.Empty);
        callback.AdPaid += (_, revenue) => OnAdPaid?.Invoke(this, revenue);
        callback.AdClicked += (s, _) => OnAdClicked?.Invoke(this, EventArgs.Empty);
        callback.AdDismissed += (s, _) => OnAdDismissed?.Invoke(this, EventArgs.Empty);

        ad.AdEventCallback = callback;

        var rewardListener = new UserEarnedRewardListener();
        rewardListener.UserEarnedReward += (s, reward) => OnUserEarnedReward?.Invoke(this, new RewardItem(reward.Amount, reward.Type));

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        ad.Show(activity, rewardListener);
    }

    private void AdLoaded(SdkIRewardedInterstitialAd rewardedInterstitialAd)
    {
        _ad = rewardedInterstitialAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
