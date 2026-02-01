using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Platforms.iOS;

namespace Plugin.AdMob;

internal partial class RewardedInterstitialAd
{
    private Google.MobileAds.RewardedInterstitialAd? _ad;

    public void Load()
    {
        // Apply global ad configuration if SDK is initialized
        var sharedInstance = MobileAds.SharedInstance;
        if (sharedInstance is not null)
        {
            sharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("WARNING: MobileAds.SharedInstance is null in RewardedInterstitialAd.Load(). SDK not initialized - ad may fail to load.");
        }
        
        var request = Request.GetDefaultRequest();

        Google.MobileAds.RewardedInterstitialAd.Load(AdUnitId, request, AdLoaded);
    }

    public void Show()
    {
        if (!IsLoaded)
        {
            return;
        }

        _ad!.Present(null, () => OnUserEarnedReward?.Invoke(this, new RewardItem(_ad.Reward.Amount.Int32Value, _ad.Reward.Type)));
    }

    private void AdLoaded(Google.MobileAds.RewardedInterstitialAd? rewardedAd, NSError? error)
    {
        if (error is not null)
        {
            OnAdFailedToLoad?.Invoke(this, new AdError(error.DebugDescription));
            return;
        }

        _ad = rewardedAd!;

        _ad.PresentedContent += (s, e) => OnAdShowed?.Invoke(s, e);
        _ad.FailedToPresentContent += (s, e) => OnAdFailedToShow?.Invoke(s, new AdError(e.Error.DebugDescription));
        _ad.RecordedImpression += (s, e) => OnAdImpression?.Invoke(s, e);
        _ad.RecordedClick += (s, e) => OnAdClicked?.Invoke(s, e);
        _ad.DismissedContent += (s, e) => OnAdDismissed?.Invoke(s, e);

        IsLoaded = true;
        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}