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

    public void LoadAndShowConsentFormIfRequired()
    {
        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        _consentInformation = UserMessagingPlatform.GetConsentInformation(activity);

        var requestParametersBuilder = new ConsentRequestParameters.Builder();

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
        _consentInformation.RequestConsentInfoUpdate(activity, requestParameters, this, this);
    }

    public bool CanRequestAds()
    {
        return _consentInformation?.CanRequestAds() ?? false;
    }

    public bool IsPrivacyOptionsRequired()
    {
        if (_consentInformation is null)
        {
            return false;
        }

        return _consentInformation.PrivacyOptionsRequirementStatus == ConsentInformationPrivacyOptionsRequirementStatus.Required;
    }

    public void ShowPrivacyOptionsForm()
    {
        var activity = ActivityStateManager.Default.GetCurrentActivity()!;
        UserMessagingPlatform.ShowPrivacyOptionsForm(activity, this);
    }

    public void Reset()
    {
        _consentInformation?.Reset();
        OnConsentInfoUpdated?.Invoke(this, GetConsentInformation());
    }

    public void OnConsentInfoUpdateSuccess()
    {
        Debug.Write($"[Plugin.AdMob] Consent info update was successful. Status: {_consentInformation!.ConsentStatus}");

        if (_consentInformation.ConsentStatus is ConsentInformationConsentStatus.Required)
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

    private ConsentInformation? GetConsentInformation()
    {
        if (_consentInformation is null)
        {
            return null;
        }

        return new((ConsentStatus)_consentInformation.ConsentStatus, _consentInformation.CanRequestAds());
    }
}
