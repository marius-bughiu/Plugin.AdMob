## Troubleshooting

This section is based on common issues seen in real projects, and on closed issues from the `Plugin.AdMob` repository.

## Choose a topic

- **iOS setup & crashes**: [Troubleshooting-iOS](Troubleshooting-iOS.md)
- **Android build / dependency issues**: [Troubleshooting-Android](Troubleshooting-Android.md)
- **Consent / UMP / GDPR**: [Troubleshooting-Consent](Troubleshooting-Consent.md)
- **Ads not showing (general)**: [Troubleshooting-Ads-not-showing](Troubleshooting-Ads-not-showing.md)
- **Banner ads**: [Troubleshooting-Banner-ads](Troubleshooting-Banner-ads.md)
- **Interstitial ads**: [Troubleshooting-Interstitial-ads](Troubleshooting-Interstitial-ads.md)
- **Native ads**: [Troubleshooting-Native-ads](Troubleshooting-Native-ads.md)
- **Blazor Hybrid**: [Troubleshooting-Blazor-Hybrid](Troubleshooting-Blazor-Hybrid.md)
- **Upgrades & target frameworks**: [Troubleshooting-Upgrades-and-target-frameworks](Troubleshooting-Upgrades-and-target-frameworks.md)

## Quick checklist

- **Android**: ensure `com.google.android.gms.ads.APPLICATION_ID` is set in `Platforms/Android/AndroidManifest.xml` and you have `INTERNET` + `ACCESS_NETWORK_STATE` permissions.
- **iOS**: ensure `GADApplicationIdentifier` is set in `Platforms/iOS/Info.plist` and that you initialize the SDK in your `AppDelegate` as described in [Setup](Setup.md).
- **Consent**: if consent is required, ads will not be requested until consent is obtained (unless you intentionally disabled consent checks).
- **New apps**: newly created / re-activated AdMob apps can take time before real ads have inventory.
