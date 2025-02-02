using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// Provides an API to manage user consent through Google's User Messaging Platform (UMP).
/// </summary>
public interface IAdConsentService
{
    /// <summary>
    /// Raised when the consent information has been updated. This does not guarantee that consent has been obtained.
    /// </summary>
    event EventHandler<IConsentInformation?>? OnConsentInfoUpdated;

    /// <summary>
    /// Raised when we failed to update the consent information.
    /// </summary>
    event EventHandler<IConsentError>? OnConsentInfoFailedToUpdate;

    /// <summary>
    /// Raised after the user has dismissed the consent form.
    /// </summary>
    event EventHandler? OnConsentFormDismissed;

    /// <summary>
    /// Raised when we fail to show the consent form.
    /// </summary>
    event EventHandler<IConsentError>? OnConsentFormError;

    /// <summary>
    /// Updates the consent information and shows the consent form if required.
    /// </summary>
    void LoadAndShowConsentFormIfRequired() => throw new NotImplementedException();

    /// <summary>
    /// Returns true when all the criteria for requesting ads is met. Doesn't take into account <see cref="AdConfig.DisableConsentCheck" />.
    /// </summary>
    bool CanRequestAds() => throw new NotImplementedException();

    /// <summary>
    /// Determines if the privacy options form is required.
    /// </summary>
    bool IsPrivacyOptionsRequired() => throw new NotImplementedException();

    /// <summary>
    /// Presents the user with the privacy options form.
    /// </summary>
    void ShowPrivacyOptionsForm() => throw new NotImplementedException();

    /// <summary>
    /// Resets the stored consent information. Should only be used during testing.
    /// </summary>
    void Reset() => throw new NotImplementedException();
}

internal partial class AdConsentService : IAdConsentService
{
    public event EventHandler<IConsentInformation?>? OnConsentInfoUpdated;
    public event EventHandler<IConsentError>? OnConsentInfoFailedToUpdate;
    public event EventHandler? OnConsentFormDismissed;
    public event EventHandler<IConsentError>? OnConsentFormError;
}
