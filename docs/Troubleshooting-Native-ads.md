## Troubleshooting (Native ads)

## Native ads don’t show, but banner works

### Symptoms

- Banner loads, but `NativeAdView` never shows anything.
- The sample works, but your app doesn’t.

### Root causes

- Consent is required and ads are not requested until consent is satisfied.
- The native ad content template is missing required assets per policy, or bindings are not wired to the `INativeAd` binding context.

### Fix / mitigation

- Verify consent flow first. During testing, consider forcing a geography and resetting consent (see [Troubleshooting-Consent](Troubleshooting-Consent.md)). This was the fix in closed issue #66.
- Ensure your `NativeAdView.AdContent` bindings target `INativeAd` properties (e.g. `Headline`, `Body`, etc.).
- Subscribe to `NativeAdView.OnAdLoaded` / `OnAdFailedToLoad` to determine whether the ad is loading or failing.

### Related issues

- [`#66`](https://github.com/marius-bughiu/Plugin.AdMob/issues/66) (native ads gated by consent)

## Native test ads fail on Android emulators without the Play Store

### Symptoms

- Native test ads fail with `Ad failed to load : 0` on an Android emulator, while banner test ads work.
- Logcat shows:

```
Received log message: <Google:HTML> Incorrect native ad response. Click actions were not properly specified
```

- The full `LoadAdError` response shows the demo campaign *was* served (`"Ad Source Instance Name": "[DevRel] [DO NOT EDIT] Native Ads Campaign"`, `"app_promotion_type": "INSTALL"`) but the SDK rejected it.

### Root cause

The native demo campaign serves app-install creatives whose click actions target the Play Store (`market://` links). On emulator images without the Play Store (**Google APIs** images, as opposed to **Google Play** images), those click actions cannot be resolved, so the SDK rejects the response.

### Fix / mitigation

- Use an emulator image that includes the Play Store (a **Google Play** system image, e.g. `system-images;android-36;google_apis_playstore;x86_64`).
- Physical devices with the Play Store are unaffected.
- On Android versions of the plugin before the native ad unit fix, also make sure you are on a plugin version where `NativeAdView` uses `AdConfig.DefaultNativeAdUnitId` (not the banner default) when no `AdUnitId` is set.

