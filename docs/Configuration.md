## Ad unit IDs

The plugin allows you to explicitly specify an `AdUnitId` for each individual view or API.

For example, for a banner ad, you can specify the `AdUnitId` directly on the view:

```
<admob:BannerAd AdUnitId="ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx" />
```

While for an interstitial ad you can specify it when creating or preparing an ad:

```
IInterstitialAd CreateAd(string? adUnitId = null);
void PrepareAd(string? adUnitId = null);
```

## Default ad unit IDs

You can define default ad unit IDs in case you want to use the same IDs all throughout the application. You can configure them using [`AdConfig`](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/src/Plugin.AdMob/Configuration/AdConfig.cs):

- `DefaultBannerAdUnitId` 
- `DefaultInterstitialAdUnitId`
- `DefaultRewardedAdUnitId`
- `DefaultRewardedInterstitialAdUnitId`
- `DefaultAppOpenAdUnitId`
- `DefaultNativeAdUnitId`

> [!TIP]
> You can also pass default ad unit IDs directly to `builder.UseAdMob(...)`. See [Setup](Setup.md).

## Test ad units

For testing purposes, you can set `AdConfig.UseTestAdUnitIds` to `true`. This will override any ad unit IDs you may have defined in your app, with [AdMob test IDs](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/src/Plugin.AdMob/Configuration/AdMobTestAdUnits.cs) which display test ads and are safe to interact with.

## Test devices

When testing on real devices, you should register the device as a test device:

```
AdConfig.AddTestDevice("<your-test-device-id>");
```

## Impression-level revenue

On Android and iOS, `OnAdPaid` reports the ad unit ID, amount, currency code, and precision when Google Mobile Ads provides revenue for an impression. Enable impression-level ad revenue in your [AdMob settings](https://support.google.com/admob/answer/11322405) first.

Subscribe before the ad can be shown. For example, to track interstitials loaded through `PrepareAd()`:

```csharp
var interstitials = IPlatformApplication.Current.Services.GetRequiredService<IInterstitialAdService>();
interstitials.OnAdPaid += (_, revenue) =>
{
    Debug.WriteLine($"{revenue.AdUnitId}: {revenue.Amount} {revenue.CurrencyCode} ({revenue.Precision})");
};
interstitials.PrepareAd();
```

Subscribe on the service for ads loaded with `PrepareAd()`, or on the returned ad instance for ads loaded with `CreateAd().Load()`. For banner and native ads displayed through a view, subscribe on `BannerAd` or `NativeAdView`. If you also have the native ad instance, subscribe on one of the two sources to count each impression once.

`Amount` is expressed in whole currency units. For example, `0.005 USD` is half a cent. Use `Precision` to distinguish SDK estimates, publisher-provided amounts, precise amounts, and amounts with unknown precision.

## Consent checks

By default the plugin will only request ads once consent requirements are satisfied.

You can opt out of consent checks by setting:
- `AdConfig.DisableConsentCheck = true`, or
- `builder.UseAdMob(disableConsentCheck: true, ...)`

> [!IMPORTANT]
> Disabling consent checks makes compliance your responsibility.

## Targeting options (COPPA / EEA under age / content rating)

The following settings affect ad requests:

- `AdConfig.TagForChildDirectedTreatment` (`TagForChildDirectedTreatment`)
- `AdConfig.TagForUnderAgeOfConsent` (`TagForUnderAgeOfConsent`)
- `AdConfig.MaxAdContentRating` (`MaxAdContentRating`)

You can set them either via `AdConfig` or directly on `UseAdMob(...)`:

```
builder.UseAdMob(
    tagForChildDirectedTreatment: TagForChildDirectedTreatment.None,
    tagForUnderAgeOfConsent: TagForUnderAgeOfConsent.None,
    maxAdContentRating: MaxAdContentRating.None);
```
