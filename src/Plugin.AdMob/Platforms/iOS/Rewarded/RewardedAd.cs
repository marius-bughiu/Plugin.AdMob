using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Platforms.iOS;

namespace Plugin.AdMob;

internal partial class RewardedAd
{
    private Google.MobileAds.RewardedAd? _ad;

    public void Load()
    {
        MobileAds.SharedInstance?.RequestConfiguration.ApplyGlobalAdConfiguration();
        var request = Request.GetDefaultRequest();

        Google.MobileAds.RewardedAd.Load(AdUnitId, request, AdLoaded);
    }

    public void Show()
    {
        if (!IsLoaded)
        {
            return;
        }

        _ad!.Present(null, () => OnUserEarnedReward?.Invoke(this, new RewardItem(_ad.AdReward.Amount.Int32Value, _ad.AdReward.Type)));
    }

    private void AdLoaded(Google.MobileAds.RewardedAd? rewardedAd, NSError? error)
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
        _ad.DismissedContent += (s, e) => OnAdDismissed?.Invoke(s, e);
        _ad.RecordedClick += (s, e) => OnAdClicked?.Invoke(s, e);

        IsLoaded = true;
        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}