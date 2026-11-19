using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Platforms.iOS;

namespace Plugin.AdMob;

internal partial class AppOpenAd
{
    private Google.MobileAds.AppOpenAd? _ad;

    public void Load() => _consentGate.WhenAllowed(LoadAd);

    private void LoadAd()
    {
        MobileAdsInitialization.Start();

        AdMobInitializer.RunWhenInitialized(() =>
        {
            MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
            var request = Request.GetDefaultRequest();

            Google.MobileAds.AppOpenAd.Load(AdUnitId, request, AdLoaded);
        }, ex => OnAdFailedToLoad?.Invoke(this, new AdError(ex)));
    }

    public void Show()
    {
        if (!TryBeginShow())
        {
            return;
        }

        _ad!.Present(null);
    }

    private void AdLoaded(Google.MobileAds.AppOpenAd? appOpenAd, NSError? error)
    {
        if (error is not null)
        {
            OnAdFailedToLoad?.Invoke(this, error.ToAdError());
            return;
        }

        _ad = appOpenAd!;

        _ad.PresentedContent += (s, e) => OnAdShowed?.Invoke(s, e);
        _ad.FailedToPresentContent += (s, e) => OnAdFailedToShow?.Invoke(s, e.Error.ToAdError(appOpenAd?.ResponseInfo?.ResponseIdentifier));
        _ad.RecordedImpression += (s, e) => OnAdImpression?.Invoke(s, e);
        _ad.PaidEventHandler = value => OnAdPaid?.Invoke(this, value.ToRevenueEventArgs(AdUnitId));
        _ad.RecordedClick += (s, e) => OnAdClicked?.Invoke(s, e);
        _ad.DismissedContent += (s, e) => OnAdDismissed?.Invoke(s, e);

        IsLoaded = true;
        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}

