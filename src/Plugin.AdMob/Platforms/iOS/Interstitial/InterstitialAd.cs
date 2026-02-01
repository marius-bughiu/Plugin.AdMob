using Foundation;
using Google.MobileAds;
using Plugin.AdMob.Platforms.iOS;

namespace Plugin.AdMob;

internal partial class InterstitialAd
{
    private Google.MobileAds.InterstitialAd? _ad;

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
            System.Diagnostics.Debug.WriteLine("WARNING: MobileAds.SharedInstance is null in InterstitialAd.Load(). Ad configuration may not be applied.");
        }
        
        var request = Request.GetDefaultRequest();

        Google.MobileAds.InterstitialAd.Load(AdUnitId, request, AdLoaded);
    }

    public void Show()
    {
        if (!IsLoaded)
        {
            return;
        }

        _ad!.Present(null);
    }

    private void AdLoaded(Google.MobileAds.InterstitialAd? interstitialAd, NSError? error)
    {
        if (error is not null)
        {
            OnAdFailedToLoad?.Invoke(this, new AdError(error.DebugDescription));
            return;
        }

        _ad = interstitialAd!;

        _ad.PresentedContent += (s, e) => OnAdShowed?.Invoke(s, e);
        _ad.FailedToPresentContent += (s, e) => OnAdFailedToShow?.Invoke(s, new AdError(e.Error.DebugDescription));
        _ad.RecordedImpression += (s, e) => OnAdImpression?.Invoke(s, e);
        _ad.RecordedClick += (s, e) => OnAdClicked?.Invoke(s, e);
        _ad.DismissedContent += (s, e) => OnAdDismissed?.Invoke(s, e);

        IsLoaded = true;
        OnAdLoaded?.Invoke(this, EventArgs.Empty);
    }
}

