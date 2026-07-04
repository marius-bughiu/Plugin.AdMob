## Troubleshooting (iOS)

## App crashes on device / TestFlight

### Symptoms

- The app runs on simulator but crashes on device/TestFlight.
- You see errors mentioning AdMob initialization or missing configuration.

### Fix / mitigation

- **Verify `Info.plist`**: ensure you have added:
  - `GADApplicationIdentifier`
  - `SKAdNetworkItems`
  - `GADIsAdManagerApp`
  - `NSUserTrackingUsageDescription` (if you request tracking permission)
- **Initialize the SDK** in your `Platforms/iOS/AppDelegate.cs`:

```
Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);
```

- If you see `GADInvalidInitializationException` mentioning initialization: ensure `GADIsAdManagerApp` exists and is set to `true` (see closed issue #21).

### Related issues

- [`#21`](https://github.com/marius-bughiu/Plugin.AdMob/issues/21) (iOS crash with `GADInvalidInitializationException`)
- [`#58`](https://github.com/marius-bughiu/Plugin.AdMob/issues/58) (TestFlight crash; typically points to iOS configuration differences between debug and distribution)

## NullReferenceException from `MobileAds.SharedInstance.Start(...)` on a physical device

### Symptoms

- `An error occurred: 'Object reference not set to an instance of an object.'` thrown from `AppDelegate.CreateMauiApp()` at the `Google.MobileAds.MobileAds.SharedInstance.Start(...)` call.
- Typically reproduces on a physical device; the simulator may appear fine.

### Root cause

The app's minimum iOS version is below what the Google Mobile Ads SDK supports (15.0). The `Jc.GMA.iOS` binding also declares this requirement.

### Fix / mitigation

Set the minimum supported iOS version to 15.0 in your app's `.csproj`:

```
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-ios'">
	<SupportedOSPlatformVersion>15.0</SupportedOSPlatformVersion>
</PropertyGroup>
```

and add the matching key to `Platforms/iOS/Info.plist` (Release builds fail without it):

```
<key>MinimumOSVersion</key>
<string>15.0</string>
```

### Related issues

- [`#56`](https://github.com/marius-bughiu/Plugin.AdMob/issues/56) (null reference on physical device when the minimum OS version is too low)

## Banner view controller cannot be detected

### Symptoms

- `System.NullReferenceException: 'The current view controller cannot be detected.'`

### Fix / mitigation

- **Update to the latest plugin version** (this was an older integration issue affecting some app structures, including Blazor Hybrid).
- If you still hit it in your app structure, a pragmatic workaround is to **delay showing ads until after startup** (for example, show a splash/landing page first and navigate to the page containing `BannerAd` after the UI is ready).

### Related issues

- [`#8`](https://github.com/marius-bughiu/Plugin.AdMob/issues/8) (root view controller not available early in startup)
- [`#35`](https://github.com/marius-bughiu/Plugin.AdMob/issues/35) (Blazor Hybrid: view controller cannot be detected)

## Interstitial does not show when a modal is open

### Symptoms

- `OnAdLoaded` fires, but the interstitial does not appear on a real iPhone.
- Works after closing the modal, or works in different UI flows.

### Fix / mitigation

- Ensure you call `ShowAd()` (or `IInterstitialAd.Show()`) when the app is presenting from a valid UI state.
- If you are displaying a MAUI modal, **dismiss the modal first**, then show the interstitial.

### Related issues

- [`#32`](https://github.com/marius-bughiu/Plugin.AdMob/issues/32) (interstitial not showing while MAUI modal is open)
