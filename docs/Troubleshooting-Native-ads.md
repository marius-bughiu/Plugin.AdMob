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

