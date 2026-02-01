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

- `#21` (iOS crash with `GADInvalidInitializationException`)
- `#58` (TestFlight crash; typically points to iOS configuration differences between debug and distribution)

## Banner view controller cannot be detected

### Symptoms

- `System.NullReferenceException: 'The current view controller cannot be detected.'`

### Fix / mitigation

- **Update to the latest plugin version** (this was an older integration issue affecting some app structures, including Blazor Hybrid).
- If you still hit it in your app structure, a pragmatic workaround is to **delay showing ads until after startup** (for example, show a splash/landing page first and navigate to the page containing `BannerAd` after the UI is ready).

### Related issues

- `#8` (root view controller not available early in startup)
- `#35` (Blazor Hybrid: view controller cannot be detected)

## Interstitial does not show when a modal is open

### Symptoms

- `OnAdLoaded` fires, but the interstitial does not appear on a real iPhone.
- Works after closing the modal, or works in different UI flows.

### Fix / mitigation

- Ensure you call `ShowAd()` (or `IInterstitialAd.Show()`) when the app is presenting from a valid UI state.
- If you are displaying a MAUI modal, **dismiss the modal first**, then show the interstitial.

### Related issues

- `#32` (interstitial not showing while MAUI modal is open)
