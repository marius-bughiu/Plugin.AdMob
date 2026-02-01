The plugin allows you to explicitly specify an `AdUnitId` for each individual view or API.

For example, for a banner ad, you can specify the `AdUnitId` directly on the view:

```
<admob:BannerAd AdUnitId="ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx" />
```

While for an interstitial ad you can specify it when creating or preparing an ad.

```
IInterstitialAd CreateAd(string adUnitId = null);
void PrepareAd(string adUnitId = null);
```

## Default ad unit IDs

You can define default ad unit IDs in case you want to use the same IDs all throughout the application. You can configure them using [`AdConfig`](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/src/Plugin.AdMob/Configuration/AdConfig.cs):

- `DefaultBannerAdUnitId` 
- `DefaultInterstitialAdUnitId`
- `DefaultRewardedAdUnitId`
- `DefaultRewardedInterstitialAdUnitId`

## Test ad units

For testing purposes, you can set `AdConfig.UseTestAdUnitIds` to `true`. This will override any ad unit IDs you may have defined in your app, with [AdMob test IDs](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/src/Plugin.AdMob/Configuration/AdMobTestAdUnits.cs) which display test ads and are safe to interact with.