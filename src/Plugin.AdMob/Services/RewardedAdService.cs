using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing rewarded ads.
/// </summary>
public interface IRewardedAdService
{
    /// <summary>
    /// Creates a rewarded ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultRewardedAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>A rewarded ad instance.</returns>
    IRewardedAd CreateAd(string adUnitId = null);

    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultRewardedAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <param name="onUserEarnedReward">A callback which is invoked when the user has earned a reward.</param>
    void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null);

    /// <summary>
    /// Displays the already prepared ad. Does nothing if no ad was prepared.
    /// </summary>
    void ShowAd();
}

internal class RewardedAdService(IAdConsentService _adConsentService) 
    : IRewardedAdService
{
    private IRewardedAd _rewardedAd;
    
    public IRewardedAd CreateAd(string adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);
        
        return new RewardedAd(adUnitId);
    }

    public void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null)
    {
        if (CanRequestAds() is false)
        {
            return;
        }

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

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService.CanRequestAds();
    }
}