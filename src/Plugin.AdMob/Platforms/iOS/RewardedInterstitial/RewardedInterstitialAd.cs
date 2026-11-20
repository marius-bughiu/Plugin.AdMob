using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Platforms.iOS;

namespace Plugin.AdMob;

internal partial class RewardedInterstitialAd
{
    private Google.MobileAds.RewardedInterstitialAd? _ad;

    public void Load() => _consentGate.WhenAllowed(LoadAd);

    private void LoadAd()
    {
        MobileAdsInitialization.Start();

        AdMobInitializer.RunWhenInitialized(() =>
        {
            MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
            var request = Request.GetDefaultRequest();

            Google.MobileAds.RewardedInterstitialAd.Load(AdUnitId, request, AdLoaded);
        }, ex => OnAdFailedToLoad?.Invoke(this, new AdError(ex)));
    }

    public void Show()
    {
        if (!TryBeginShow())
        {
            return;
        }

        _ad!.Present(null, () => OnUserEarnedReward?.Invoke(this, new RewardItem(_ad.Reward.Amount.Int32Value, _ad.Reward.Type)));
    }

    private void AdLoaded(Google.MobileAds.RewardedInterstitialAd? rewardedAd, NSError? error)
    {
        if (error is not null)
        {
            OnAdFailedToLoad?.Invoke(this, error.ToAdError());
            return;
        }

        // A previously loaded ad may already be presenting. Keep its paid callback
        // available for the impression in flight, and clear it for unshown ads.
        if (IsLoaded && _ad is not null)
        {
            _ad.PaidEventHandler = null;
        }

        _ad = rewardedAd!;
        var responseId = _ad.ResponseInfo?.ResponseIdentifier;

        _ad.PresentedContent += (s, e) => OnAdShowed?.Invoke(s, e);
        _ad.FailedToPresentContent += (s, e) => OnAdFailedToShow?.Invoke(s, e.Error.ToAdError(responseId));
        _ad.RecordedImpression += (s, e) => OnAdImpression?.Invoke(s, e);
        _ad.PaidEventHandler = PaidEventHandlers.Create(this, static (ad, value) =>
            ad.OnAdPaid?.Invoke(ad, value.ToRevenueEventArgs(ad.AdUnitId)));
        _ad.RecordedClick += (s, e) => OnAdClicked?.Invoke(s, e);
        _ad.DismissedContent += (s, e) => OnAdDismissed?.Invoke(s, e);

        IsLoaded = true;
        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}
