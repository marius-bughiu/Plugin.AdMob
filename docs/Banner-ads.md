## Displaying a banner ad

Add the controls namespace at the top of your page:

```
xmlns:admob="clr-namespace:Plugin.AdMob;assembly=Plugin.AdMob"
```

and then place the banner in your page.

```
<admob:BannerAd AdUnitId="ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx" />
```

> [!NOTE]
> The `AdUnitId` is optional when using test ads. You can enable test ads by setting `AdConfig.UseTestAdUnitIds` to `true`.

### Supported sizes

| AdSize | Width | Height |
|---|---|---|
| SmartBanner | Screen width | 32/50/90 |
| Banner | 320 | 50 |
| LargeBanner | 320 | 100 |
| MediumRectangle | 300 | 250 |
| FullBanner | 468 | 60 |
| Leaderboard | 728 | 90 |
| Custom | * | * |

### Custom size ad

You can also create banners with a custom size by specifying `AdSize="Custom"` in combination with `CustomAdWidth` and `CustomAdHeight`. For example:

```
<admob:BannerAd AdSize="Custom" CustomAdWidth="200" CustomAdHeight="200" /> 
```

## Properties
| Property | Description |
|---|---|
| AdUnitId | The ad unit id. |
| AdSize | The desired ad size. |
| CustomAdWidth | The desired ad width in density-independent pixels. Used when `AdSize` is set to `AdSize.Custom`. |
| CustomAdHeight | The desired ad height in density-independent pixels. Used when `AdSize` is set to `AdSize.Custom`. |
| IsLoaded | Determines whether the ad is loaded or not. |

## Events
| Event | Description |
|---|---|
| OnAdLoaded | Raised when an ad is received. |
| OnAdFailedToLoad | Raised when an ad request failed. |
| OnAdImpression | Raised when an impression is recorded for an ad. |
| OnAdClicked | Raised when a click is recorded for an ad. |
| OnAdSwiped | Raised when a swipe gesture on an ad is recorded as a click. Supported only by Android. |
| OnAdOpened | Raised when an ad opens an overlay that covers the screen. |
| OnAdClosed | Raised when the user is about to return to the application after clicking on an ad. |
