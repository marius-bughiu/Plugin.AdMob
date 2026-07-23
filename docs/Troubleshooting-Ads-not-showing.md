## Troubleshooting (Ads not showing)

## Only test ads show, or real ads never show in release

### Symptoms

- Test ads show, but real ads do not.
- Real ads do not show immediately after creating or re-activating an AdMob app/account.

### Root cause

For new apps / new or re-activated AdMob accounts, it can take time for real ads to start serving (inventory / account readiness).

### Fix / mitigation

- Verify you are using real **Ad Unit IDs** (not Google test IDs).
- Ensure `AdConfig.UseTestAdUnitIds` is `false` when testing real ads.
- Wait and test again later if the AdMob app/account is new or was recently re-activated.

### Related issues

- [`#11`](https://github.com/marius-bughiu/Plugin.AdMob/issues/11) (real ads not showing; resolved after AdMob account/app became ready)

## "No ad to show" / "No fill" — the request succeeds but no ad appears

### Symptoms

- Ads (banner, interstitial, native, ...) don't appear, with no obvious error.
- It can affect **every** ad format at once, and can happen even with Google's **test/demo ad unit IDs**.

### Root cause

The ad request reached Google's servers and Google returned **no ad to serve** — a *no-fill*. This is a serving/inventory condition on Google's side, **not** a bug in your app or the plugin. It is common and expected transiently, including on Google's demo ad units.

### How to verify

There are two ways to see the failure — the plugin's own log, and the `OnAdFailedToLoad` event.

**Option A — the plugin's debug log.** In **Debug** builds, the plugin writes every ad error to the debugger output:

```
[Plugin.AdMob] Ad error: Request Error: No ad to show. ...
```

Look for `[Plugin.AdMob] Ad error:` lines in your IDE's debug output. Note two limits: this line is compiled out of **Release** builds, and debugger output is **not** captured on a physical device unless a debugger is attached (so it won't appear in a raw device console) — for those cases use Option B.

**Option B — handle `OnAdFailedToLoad`.** Every ad type raises `OnAdFailedToLoad` (an `EventHandler<IAdError>`); `IAdError.Message` contains Google's full native error text. This works in Release and on-device. For a banner in XAML:

```xml
<admob:BannerAd OnAdFailedToLoad="Banner_OnAdFailedToLoad" />
```
```csharp
private void Banner_OnAdFailedToLoad(object sender, IAdError e)
    => Console.WriteLine($"[AdMob] load failed: {e.Message}");
```

The same `OnAdFailedToLoad` event exists on `InterstitialAd`, `RewardedAd`, `RewardedInterstitialAd`, `AppOpenAd`, and `NativeAdView` / `INativeAd`. Use `Console.WriteLine` (not `Debug.WriteLine`) if you need the output to reach a physical device's console.

**Read the message.** A no-fill looks like:

- **iOS:** `Request Error: No ad to show.` (`Error Domain=com.google.admob Code=1`)
- **Android:** `No fill.` (error code `3`)

Crucially, the error's response info includes a **`Response ID`** (e.g. `Response ID: Q8pfav...`). A populated Response ID confirms the request completed a round-trip to Google and Google chose not to serve — so it is a genuine no-fill, **not** a network block, a missing `GADApplicationIdentifier`, or a consent problem (a DNS/ad blocker would fail before any Response ID, and a consent block would keep `CanRequestAds()` `false` so no request is issued at all).

### Fix / mitigation

- **Retry later.** Demo/test no-fills usually clear on their own within hours; add retry-with-backoff for production.
- **Confirm consent first:** `IAdConsentService.CanRequestAds()` must be `true` before a request is even sent (see [Consent](Consent.md)).
- **For real ad units:** new or low-traffic apps, restricted regions, or limited/non-personalized consent all reduce fill; a demo-unit no-fill does not affect your real units, so test those separately.

## Full-screen test ads fail on an Android emulator (banners work)

Interstitial, rewarded, and app open test ads can fail with `Ad failed to load : 3` ("No fill.") on emulators running with software GPU rendering, while banner test ads load fine. Run the emulator with hardware graphics acceleration. See [Troubleshooting-Android](Troubleshooting-Android.md#full-screen-test-ads-fail-with-no-fill-on-emulators-banners-work) for details.

## Ads not loading in sample or on real device

### Symptoms

- Sample app runs but ads don’t load on a real device.

### Fix / mitigation

- Register your device as a test device during testing:

```
AdConfig.AddTestDevice("<your-device-id>");
```

- Prefer using `AdConfig.UseTestAdUnitIds = true` while integrating.

### Related issues

- [`#6`](https://github.com/marius-bughiu/Plugin.AdMob/issues/6) (sample app + device testing guidance)
