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
