using Android.Gms.Ads;
using System.Diagnostics;
using Xamarin.Google.UserMesssagingPlatform;

namespace Plugin.AdMob.Services;

internal partial class AdConsentService :
    AdLoadCallback,
    IConsentFormOnConsentFormDismissedListener,
    IConsentInformationOnConsentInfoUpdateFailureListener,
    IConsentInformationOnConsentInfoUpdateSuccessListener
{
    Xamarin.Google.UserMesssagingPlatform.IConsentInformation? _consentInformation;

    private Xamarin.Google.UserMesssagingPlatform.IConsentInformation ConsentInformation =>
        _consentInformation ??= UserMessagingPlatform.GetConsentInformation(Android.App.Application.Context);

    public void LoadAndShowConsentFormIfRequired()
    {
        var activity = ActivityStateManager.Default.GetCurrentActivity()!;

        var requestParametersBuilder = new ConsentRequestParameters.Builder()
            .SetTagForUnderAgeOfConsent(Configuration.AdConfig.TagForUnderAgeOfConsent is TagForUnderAgeOfConsent.True);

        var ds = Configuration.ConsentDebugSettings.Current;
        if (ds is not null)
        {
            if (ds.Reset)
            {
                Reset();
            }

            var debugSettingsBuilder = new ConsentDebugSettings.Builder(activity);
            debugSettingsBuilder.SetDebugGeography((int)ds.Geography);

            foreach (var testDeviceHashedId in ds.TestDeviceHashedIds)
            {
                debugSettingsBuilder.AddTestDeviceHashedId(testDeviceHashedId);
            }

            var debugSettings = debugSettingsBuilder.Build();

            requestParametersBuilder.SetConsentDebugSettings(debugSettings);
        }

        var requestParameters = requestParametersBuilder.Build();
        ConsentInformation.RequestConsentInfoUpdate(activity, requestParameters, this, this);

        if (CanRequestAds())
        {
            OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
        }
    }

    public bool CanRequestAds()
    {
        return ConsentInformation.CanRequestAds();
    }

    public bool IsPrivacyOptionsRequired()
    {
        return ConsentInformation.PrivacyOptionsRequirementStatus == ConsentInformationPrivacyOptionsRequirementStatus.Required;
    }

    public void ShowPrivacyOptionsForm()
    {
        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        UserMessagingPlatform.ShowPrivacyOptionsForm(activity, this);
    }

    public void Reset()
    {
        ConsentInformation.Reset();
        OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
    }

    public void OnConsentInfoUpdateSuccess()
    {
        Debug.Write($"[Plugin.AdMob] Consent info update was successful. Status: {ConsentInformation.ConsentStatus}");

        if (ConsentInformation.ConsentStatus is ConsentInformationConsentStatus.Required)
        {
            var activity = ActivityStateManager.Default.GetCurrentActivity()!;
            UserMessagingPlatform.LoadAndShowConsentFormIfRequired(activity, this);

            return;
        }

        OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
    }

    public void OnConsentInfoUpdateFailure(FormError error)
    {
        Debug.Write($"[Plugin.AdMob] Consent info update failed: {error.Message}");
        OnConsentInfoFailedToUpdate?.Invoke(this, new ConsentError(error.Message));

        if (CanRequestAds())
        {
            OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
        }
    }

    void IConsentFormOnConsentFormDismissedListener.OnConsentFormDismissed(FormError? error)
    {
        if (error is not null)
        {
            Debug.Write($"[Plugin.AdMob] Consent form error: {error.Message}");
            OnConsentFormError?.Invoke(this, new ConsentError(error.Message));
            return;
        }

        OnConsentFormDismissed?.Invoke(this, EventArgs.Empty);
        OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
    }

    private ConsentInformation GetConsentInformation() => new((ConsentStatus)ConsentInformation.ConsentStatus, ConsentInformation.CanRequestAds());
}
