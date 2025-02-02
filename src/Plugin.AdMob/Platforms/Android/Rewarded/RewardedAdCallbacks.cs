using Android.Gms.Ads;
using Android.Gms.Ads.Rewarded;

namespace Plugin.AdMob.Rewarded;

internal sealed class RewardedAdCallbacks : Android.Gms.Ads.Workarounds.RewardedAdLoadCallback, IOnUserEarnedRewardListener
{
    public event EventHandler<global::Android.Gms.Ads.Rewarded.RewardedAd>? WhenAdLoaded;
    public event EventHandler<LoadAdError>? WhenAdFailedToLoad;
    public event EventHandler<IRewardItem>? WhenUserEarnedReward;

    public override void OnAdLoaded(global::Android.Gms.Ads.Rewarded.RewardedAd rewardedAd)
    {
        base.OnAdLoaded(rewardedAd);
        WhenAdLoaded?.Invoke(this, rewardedAd);
    }

    public override void OnAdFailedToLoad(LoadAdError error)
    {
        base.OnAdFailedToLoad(error);
        WhenAdFailedToLoad?.Invoke(this, error);
    }

    public void OnUserEarnedReward(IRewardItem reward)
    {
        WhenUserEarnedReward?.Invoke(this, reward);
    }
}