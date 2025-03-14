using Android.Gms.Ads;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.RewardedInterstitial;

namespace Plugin.AdMob;

internal partial class RewardedInterstitialAd
{
    private Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd? _ad;
    private RewardedInterstitialAdCallbacks? _callbacks;

    public void Load()
    {
        var configBuilder = new RequestConfiguration.Builder();
        configBuilder.SetTestDeviceIds(AdConfig.TestDevices);
        MobileAds.RequestConfiguration = configBuilder.Build();

        var requestBuilder = new AdRequest.Builder();
        var adRequest = requestBuilder.Build();

        _callbacks = new RewardedInterstitialAdCallbacks();
        _callbacks.WhenAdLoaded += AdLoaded;
        _callbacks.WhenAdFailedToLoad += (s, e) => OnAdFailedToLoad?.Invoke(s, new AdError(e.Message));
        _callbacks.WhenUserEarnedReward += (s, e) => OnUserEarnedReward?.Invoke(s, new RewardItem(e.Amount, e.Type));

        Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd.Load(Android.App.Application.Context, AdUnitId, adRequest, _callbacks);
    }
    
    private void AdLoaded(object? sender, Android.Gms.Ads.RewardedInterstitial.RewardedInterstitialAd rewardedInterstitialAd)
    {
        _ad = rewardedInterstitialAd;
        IsLoaded = true;

        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void Show()
    {
        if (!IsLoaded)
        {
            return;
        }
        
        var listener = new FullScreenContentCallback();
        
        listener.AdShowed += (s, _) => OnAdShowed?.Invoke(s, EventArgs.Empty);
        listener.AdFailedToShow += (s, e) => OnAdFailedToShow?.Invoke(s, new AdError(e.Message));
        listener.AdImpression += (s, _) => OnAdImpression?.Invoke(s, EventArgs.Empty);
        listener.AdClicked += (s, _) => OnAdClicked?.Invoke(s, EventArgs.Empty);
        listener.AdDismissed += (s, _) => OnAdDismissed?.Invoke(s, EventArgs.Empty);

        _ad!.FullScreenContentCallback = listener;

        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        _ad.Show(activity, _callbacks!);
    }
}