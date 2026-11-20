using Android.Runtime;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Plugin.AdMob.Platforms.Android;
using SdkIRewardedAd = Google.Android.Libraries.Ads.Mobile.Sdk.Rewarded.IRewardedAd;
using SdkRewardedAd = Google.Android.Libraries.Ads.Mobile.Sdk.Rewarded.RewardedAd;

namespace Plugin.AdMob;

internal partial class RewardedAd
{
    private SdkIRewardedAd? _ad;

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
            callback.Loaded += (s, ad) => AdLoaded(ad.JavaCast<SdkIRewardedAd>()!);
            callback.Failed += (_, error) => OnAdFailedToLoad?.Invoke(this, error.ToAdError());

            SdkRewardedAd.Load(adRequest, callback);
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
        callback.AdShowed += (s, _) => OnAdShowed?.Invoke(this, EventArgs.Empty);
        callback.AdFailedToShow += (_, error) => OnAdFailedToShow?.Invoke(this, error.ToAdError(responseId));
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

    private void AdLoaded(SdkIRewardedAd rewardedAd)
    {
        _ad = rewardedAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
