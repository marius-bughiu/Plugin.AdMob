using Android.Gms.Ads;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Rewarded;

namespace Plugin.AdMob;

internal partial class RewardedAd
{
    private Android.Gms.Ads.Rewarded.RewardedAd? _ad;
    private RewardedAdCallbacks? _callbacks;

    public void Load()
    {
        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);
        MobileAds.RequestConfiguration = configBuilder.Build();
        
        var requestBuilder = new AdRequest.Builder();
        var adRequest = requestBuilder.Build();
        
        _callbacks = new RewardedAdCallbacks();
        _callbacks.WhenAdLoaded += AdLoaded;
        _callbacks.WhenAdFailedToLoad += (s, e) => OnAdFailedToLoad?.Invoke(s, new AdError(e.Message));
        _callbacks.WhenUserEarnedReward += (s, e) => OnUserEarnedReward?.Invoke(s, new RewardItem(e.Amount, e.Type));

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        Android.Gms.Ads.Rewarded.RewardedAd.Load(activity, AdUnitId, adRequest, _callbacks);
    }

    public void Show()
    {
        if (!IsLoaded || _ad is null)
        {
            return;
        }
        
        var listener = new FullScreenContentCallback();
        
        listener.AdPresented += (s, _) => OnAdShowed?.Invoke(s, EventArgs.Empty);
        listener.AdFailedToPresent += (s, e) => OnAdFailedToShow?.Invoke(s, new AdError(e.Message));
        listener.AdImpression += (s, _) => OnAdImpression?.Invoke(s, EventArgs.Empty);
        listener.AdClicked += (s, _) => OnAdClicked?.Invoke(s, EventArgs.Empty);
        listener.AdClosed += (s, _) => OnAdDismissed?.Invoke(s, EventArgs.Empty);

        _ad.FullScreenContentCallback = listener;

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        _ad.Show(activity, _callbacks!);
    }
    
    private void AdLoaded(object? sender, global::Android.Gms.Ads.Rewarded.RewardedAd rewardedAd)
    {
        _ad = rewardedAd;
        IsLoaded = true;
        
        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}