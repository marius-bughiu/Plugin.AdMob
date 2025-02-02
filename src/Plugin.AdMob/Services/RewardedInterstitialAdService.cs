using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing rewarded interstitial ads.
/// </summary>
public interface IRewardedInterstitialAdService
{
    /// <summary>
    /// Creates a rewarded interstitial ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultRewardedInterstitialAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>A rewarded interstitial ad instance.</returns>
    IRewardedInterstitialAd CreateAd(string adUnitId = null);
    
    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultRewardedInterstitialAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <param name="onUserEarnedReward">A callback which is invoked when the user has earned a reward.</param>
    void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null);
    
    /// <summary>
    /// Displays the already prepared ad. Does nothing if no ad was prepared.
    /// </summary>
    void ShowAd();
}

internal class RewardedInterstitialAdService(IAdConsentService _adConsentService) 
    : IRewardedInterstitialAdService
{
    private IRewardedInterstitialAd _rewardedInterstitialAd;
    
    public IRewardedInterstitialAd CreateAd(string adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);
        
        return new RewardedInterstitialAd(adUnitId);
    }
    
    public void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null)
    {
        if (CanRequestAds() is false)
        {
            return;
        }

        var rewardedInterstitialAd = CreateAd(adUnitId);
        
        if (onUserEarnedReward != null)
        {
            rewardedInterstitialAd.OnUserEarnedReward += (_, reward) => onUserEarnedReward(reward);
        }
        rewardedInterstitialAd.Load();
        
        _rewardedInterstitialAd = rewardedInterstitialAd;
    }
    
    public void ShowAd()
    {
        _rewardedInterstitialAd?.Show();
    }
    
    private string GetAdUnitId(string adUnitId)
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.RewardedInterstitial;
        }

        return adUnitId ?? AdConfig.DefaultRewardedInterstitialAdUnitId;
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