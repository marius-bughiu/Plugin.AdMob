## Troubleshooting (Interstitial ads)

## Tapping a button does nothing (no interstitial shows)

### Symptoms

- You call `ShowAd()` but nothing is shown.
- It works sometimes, especially if you tap twice.

### Root cause

An interstitial only shows after it has loaded. If you call `ShowAd()` while no ad is loaded, nothing will happen.

### Fix / mitigation

- Use `IInterstitialAdService.IsAdLoaded` (or `IInterstitialAd.IsLoaded`) and/or subscribe to `OnAdLoaded` before calling `ShowAd()`.
- If you need reliable error handling, use `CreateAd(...)` and subscribe to `OnAdFailedToLoad` / `OnAdFailedToShow`.

### Related issues

- [`#31`](https://github.com/marius-bughiu/Plugin.AdMob/issues/31) (request for load state tracking; the service now exposes `IsAdLoaded`)

## Test interstitials fail with "no fill" on an Android emulator

Full-screen test ads (interstitial, rewarded, app open) can fail with `Ad failed to load : 3` on emulators running with software GPU rendering, while banner test ads work fine. See [Troubleshooting-Android](Troubleshooting-Android.md#full-screen-test-ads-fail-with-no-fill-on-emulators-banners-work).

## Interstitial does not show while a MAUI modal is open (iOS)

### Symptoms

- Banner works, but interstitial doesn’t show while a modal is open.

### Fix / mitigation

- Dismiss the modal first, then show the interstitial.

### Related issues

- [`#32`](https://github.com/marius-bughiu/Plugin.AdMob/issues/32) (interstitial not showing while modal is open)
