using Foundation;
using Google.UserMessagingPlatform;
using Plugin.AdMob.Configuration;
using System.Diagnostics;

namespace Plugin.AdMob.Services;

internal partial class AdConsentService
{
    internal Google.UserMessagingPlatform.ConsentInformation ConsentInformation => Google.UserMessagingPlatform.ConsentInformation.SharedInstance;

    internal Google.UserMessagingPlatform.ConsentStatus ConsentStatus => ConsentInformation?.ConsentStatus ?? Google.UserMessagingPlatform.ConsentStatus.Unknown;

    public void LoadAndShowConsentFormIfRequired()
    {
        if (ConsentInformation is null)
        {
            return;
        }

        if (ConsentStatus is Google.UserMessagingPlatform.ConsentStatus.NotRequired ||
            ConsentStatus is Google.UserMessagingPlatform.ConsentStatus.Obtained)
        {
            return;
        }

        RequestParameters requestParameters = new();

        var ds = ConsentDebugSettings.Current;
        if (ds is not null)
        {
            if (ds.Reset)
            {
                Reset();
            }

            requestParameters.DebugSettings = new()
            {
                TestDeviceIdentifiers = [.. ds.TestDeviceHashedIds],
                Geography = (DebugGeography)ds.Geography
            };
        };

        Google.UserMessagingPlatform.ConsentInformation.SharedInstance.RequestConsentInfoUpdateWithParameters(requestParameters, error =>
        {
            if (error is not null)
            {
                Debug.Write($"[Plugin.AdMob] Consent info update failed: {error.DebugDescription}");
                OnConsentInfoFailedToUpdate?.Invoke(this, new ConsentError(error.DebugDescription));
                return;
            }

            Debug.Write($"[Plugin.AdMob] Consent info update was successful. Status: {ConsentStatus}");

            if (ConsentStatus is Google.UserMessagingPlatform.ConsentStatus.Required)
            {
                var viewController = Platform.GetCurrentUIViewController();
                ConsentForm.LoadAndPresentIfRequiredFromViewController(viewController, OnConsentFormDismissedInternal);
                return;
            }

            OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
        });
    }

    public bool CanRequestAds()
    {
        return ConsentInformation?.CanRequestAds ?? false;
    }

    public bool IsPrivacyOptionsRequired()
    {
        return ConsentInformation.PrivacyOptionsRequirementStatus == UMPPrivacyOptionsRequirementStatus.Requried;
    }

    public void ShowPrivacyOptionsForm()
    {
        var viewController = Platform.GetCurrentUIViewController();
        ConsentForm.PresentPrivacyOptionsFormFromViewController(viewController, OnConsentFormDismissedInternal);
    }

    public void Reset()
    {
        ConsentInformation.Reset();
        OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
    }

    private void OnConsentFormDismissedInternal(NSError? error)
    {
        if (error is not null)
        {
            Debug.Write($"[Plugin.AdMob] Consent form error: {error.DebugDescription}");
            OnConsentFormError?.Invoke(this, new ConsentError(error.DebugDescription));
            return;
        }

        OnConsentFormDismissed?.Invoke(this, EventArgs.Empty);
        OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
    }

    private ConsentInformation GetConsentInformation() => new((ConsentStatus)ConsentInformation.ConsentStatus, ConsentInformation.CanRequestAds);
}
