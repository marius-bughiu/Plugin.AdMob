using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

internal sealed class ConsentGate(IAdConsentService adConsentService)
{
    public bool CanRequestAds() => AdConfig.DisableConsentCheck || adConsentService.CanRequestAds();

    public void WhenAllowed(Action action)
    {
        if (CanRequestAds())
        {
            action();
            return;
        }

        void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
        {
            if (CanRequestAds())
            {
                adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
                action();
            }
        }

        adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
    }
}
