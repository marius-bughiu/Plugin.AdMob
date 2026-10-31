using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing rewarded ads.
/// </summary>
public interface IRewardedAdService
{
    /// <summary>
    /// True when an ad is ready to be presented to the user. Turns false once the ad is shown or expires.
    /// </summary>
    public bool IsAdLoaded { get; }

    /// <summary>
    /// Creates a rewarded ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultRewardedAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>A rewarded ad instance.</returns>
    IRewardedAd CreateAd(string? adUnitId = null);

    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultRewardedAdUnitId" /> will be used. If consent doesn't allow ad requests yet, the ad is loaded as soon as it does.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <param name="onUserEarnedReward">A callback which is invoked when the user has earned a reward.</param>
    void PrepareAd(string? adUnitId = null, Action<RewardItem>? onUserEarnedReward = null);

    /// <summary>
    /// Displays the already prepared ad. Does nothing if no ad was prepared.
    /// </summary>
    void ShowAd();

    /// <summary>
    /// Raised when an ad is loaded, after calling <see cref="PrepareAd" />. 
    /// You can now call <see cref="ShowAd" /> to present the ad to the user.
    /// Note: This is not a catch-all event handler. When using <see cref="CreateAd" />, you should register to the ad loaded
    /// event handler of the IRewardedAd returned by the method.
    /// </summary>
    event EventHandler OnAdLoaded;
}

internal class RewardedAdService(IAdConsentService _adConsentService) 
    : IRewardedAdService
{
    private IRewardedAd? _rewardedAd;
    private string? _pendingAdUnitId;
    private Action<RewardItem>? _pendingOnUserEarnedReward;

    public bool IsAdLoaded => _rewardedAd?.IsLoaded ?? false;

    public event EventHandler? OnAdLoaded;

    public IRewardedAd CreateAd(string? adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default rewarded ad unit ID has been configured.");
        }

        return new RewardedAd(adUnitId);
    }

    public void PrepareAd(string? adUnitId = null, Action<RewardItem>? onUserEarnedReward = null)
    {
        _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;

        if (_rewardedAd is not null)
        {
            _rewardedAd.OnAdLoaded -= OnAdLoadedInternal;
            _rewardedAd = null;
        }

        if (CanRequestAds() is false)
        {
            _pendingAdUnitId = adUnitId;
            _pendingOnUserEarnedReward = onUserEarnedReward;
            _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
            return;
        }

        var rewardedAd = CreateAd(adUnitId);
        rewardedAd.OnAdLoaded += OnAdLoadedInternal;

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
    
    private static string? GetAdUnitId(string? adUnitId)
    {
#if ANDROID || IOS
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Rewarded;
        }
#endif

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

    private void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
    {
        if (CanRequestAds())
        {
            PrepareAd(_pendingAdUnitId, _pendingOnUserEarnedReward);
        }
    }

    private void OnAdLoadedInternal(object? sender, EventArgs e)
    {
        OnAdLoaded?.Invoke(sender, e);
    }
}