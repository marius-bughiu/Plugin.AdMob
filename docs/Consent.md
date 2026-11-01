Starting with version `1.3.0` of the plugin, consent is handled automatically using Google's User Messaging Platform (UMP). IF REQUIRED, users will be prompted with the consent form on application startup, and ad requests will only be made once consent has been obtained.

> [!NOTE]
> For consent to work, you need to configure all the dialogs in AdMob’s *Privacy & messaging* section.

## iOS: UMP, IDFA, and ATT

Google's User Messaging Platform (UMP) and Apple's App Tracking Transparency (ATT) serve different purposes:

- UMP gathers privacy choices required by regulations such as GDPR and controls whether ads can be requested.
- ATT is the iOS system permission for tracking and access to the IDFA.

If your app requests tracking permission, create and publish an **IDFA message** for the same app in AdMob's *Privacy & messaging* section. Also publish any GDPR or other regional messages your app needs. When both GDPR and IDFA messages are enabled, UMP decides which messages are required and their order based on the user's location and choices. See Google's [IDFA message guide](https://developers.google.com/admob/ios/privacy/idfa) and [message availability](https://support.google.com/admob/answer/10114014).

With automatic consent handling enabled, use this flow:

1. Configure and publish the required messages in AdMob.
2. Add `NSUserTrackingUsageDescription` to `Platforms/iOS/Info.plist` as described in [Setup](Setup.md#ios-setup).
3. Call `UseAdMob()` and let the plugin start the UMP flow.
4. UMP presents the IDFA explainer, followed by the native ATT prompt when the authorization status is not yet determined.

Don't also call `ATTrackingManager.RequestTrackingAuthorization` yourself when using an AdMob IDFA message. A separate or earlier request can reverse the intended message order or make a later consent form look like a second request to track.

### When the user denies tracking

ATT denial does not mean that all ads must be disabled. Google states that ads can still be requested without the IDFA. The plugin continues once UMP reports that ads can be requested, while the Google Mobile Ads SDK omits the IDFA from its requests.

Don't ask the user to allow tracking again in the same flow, gate app functionality on ATT approval, or treat a GDPR choice as overriding an ATT denial. Any separate regional privacy message must clearly describe its own data use and remain consistent with the ATT choice. Apple explains these requirements in [User Privacy and Data Use](https://developer.apple.com/app-store/user-privacy-and-data-use/).

### Testing the complete flow

ATT is a system permission and its prompt normally appears only once per installation. `IAdConsentService.Reset()` and `ConsentDebugSettings.Reset` reset UMP state for testing, but they don't reset the iOS ATT decision. Uninstall and reinstall the app to test the ATT prompt again.

Before shipping, test a clean **Release** installation, not only a Debug build:

- Confirm the IDFA explainer appears before the native ATT prompt.
- Test both **Allow** and **Ask App Not to Track**.
- Confirm ads can still load after ATT denial, without showing another tracking request.
- Force EEA geography on a test device to verify the combined GDPR and IDFA flow.
- Confirm that the messages are published for the `GADApplicationIdentifier` used by the build.

Use test ad unit IDs and consent debug settings only on registered test devices. Remove reset and forced-geography options from production builds.

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
