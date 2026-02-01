Starting with version `1.3.0` of the plugin, consent is handled automatically using Google's User Messaging Platform (UMP). IF REQUIRED, users will be prompted with the consent form on application startup, and ad requests will only be made once consent has been obtained.

> [!NOTE]
> For consent to work, you need to configure all the dialogs in AdMob’s *Privacy & messaging* section.

## Opting out

The `UseAdMob` method allows you to opt out of automatic consent handling in one of two ways:
- `automaticallyAskForConsent: false` - by setting this parameter to `false`, we will no longer prompt the user for consent on application startup. Asking the user for consent becomes the developer's responsibility. Asking for consent can be done using `IAdConsentService.LoadAndShowConsentFormIfRequired()`. Ad requests will not be made until consent has been obtained from the user.
- `disableConsentCheck: true` - by setting this parameter to `true`, you disable all the consent checks made before ad requests. It becomes your responsibility as a developer to make sure consent has been obtained before making ad requests, so that you are in compliance with regulations.

Consent check can also be managed using `AdConfig.DisableConsentCheck`.

## Forcing a geography

The plugin provides a bootstrapper method which allows you to test your app's behavior as though the device were located in various regions, such as the EEA or UK. Note that debug settings only work on test devices.

```
builder
    .UseAdMob()
    .UseConsentDebugSettings(...)
```

`UseConsentDebugSettings` takes in a `ConsentDebugSettings` object, with the following properties:
- `Geography` - you can set this to `Disabled` / `Eea` / `RegulatedUsState` / `Other`.
- `TestDeviceHashedIds` - when testing on an actual device, you will need to register it as a test device. Alternatively, you can use `AddTestDeviceHashedId(...)`.
- `Reset` - when set to `true`, the consent information will be reset on each application start.

## Advanced

If you opt-out from automatic consent handling, you can make use of the `IAdConsentService` service to manage the user's consent. The service is registered by the plugin and can be either injected or retrieved from the service provider as follows:

```
var adConsentService = IPlatformApplication.Current.Services.GetRequiredService<IAdConsentService>();
```

### Interface
```
public interface IAdConsentService
{
    event EventHandler<IConsentInformation?>? OnConsentInfoUpdated;

    event EventHandler<IConsentError> OnConsentInfoFailedToUpdate;

    event EventHandler OnConsentFormDismissed;

    event EventHandler<IConsentError> OnConsentFormError;

    void LoadAndShowConsentFormIfRequired();

    bool CanRequestAds();

    bool IsPrivacyOptionsRequired();

    void ShowPrivacyOptionsForm();

    void Reset();
}
```

### Methods

#### LoadAndShowConsentFormIfRequired
`void LoadAndShowConsentFormIfRequired()`

Updates the consent information and shows the consent form if required.

#### CanRequestAds
`bool CanRequestAds()`

Returns `true` when all the criteria for requesting ads is met. Doesn't take into account `AdConfig.DisableConsentCheck`.

#### IsPrivacyOptionsRequired
`bool IsPrivacyOptionsRequired()`

Determines if the privacy options form is required.

#### ShowPrivacyOptionsForm
`void ShowPrivacyOptionsForm()`

Presents the user with the privacy options form.

#### Reset
`void Reset()`

Resets the stored consent information. Should only be used during testing.

### Events

#### OnConsentInfoUpdated
`event EventHandler<IConsentInformation?>? OnConsentInfoUpdated`

Raised when the consent information has been updated. This does not guarantee that consent has been obtained. `IConsentInformation` event argument will include up to date consent information.

#### OnConsentInfoFailedToUpdate
`event EventHandler<IConsentError> OnConsentInfoFailedToUpdate`

Raised when we failed to update the consent information. `IConsentError` will contain error details.

#### OnConsentFormDismissed
`event EventHandler OnConsentFormDismissed`

Raised after the user has dismissed the consent form.

#### OnConsentFormError
`event EventHandler<IConsentError> OnConsentFormError`

Raised when we fail to show the consent form. `IConsentError` will contain error details.