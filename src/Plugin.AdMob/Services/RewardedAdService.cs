using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

public interface IRewardedAdService
{
    IRewardedAd CreateAd(string adUnitId = null);
    
    void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null);
    
    void ShowAd();
}

internal class RewardedAdService : IRewardedAdService
{
    private IRewardedAd _rewardedAd;
    
    public IRewardedAd CreateAd(string adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);
        
        return new RewardedAd(adUnitId);
    }

    public void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null)
    {
        var rewardedAd = CreateAd(adUnitId);
        
        if (onUserEarnedReward != null)
        {
            rewardedAd.OnUserEarnedReward += (_, reward) => onUserEarnedReward(reward);
        }
        rewardedAd.Load();
        
        _rewardedAd = rewardedAd;
    }

    public void ShowAd()
    {
        _rewardedAd?.Show();
    }
    
    private string GetAdUnitId(string adUnitId)
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.RewardedVideo;
        }

        return adUnitId ?? AdConfig.DefaultRewardedAdUnitId;
    }
}