Interstitial ads are displayed using the `IInterstitialAdService` service. The service is registered by the plugin and can be either injected or retrieved from the service provider as follows:

```
var interstitialAdService = IPlatformApplication.Current.Services.GetRequiredService<IInterstitialAdService>();
```

Once you grab hold of the service instance, the next step is to preload the interstitial ad. You can do so by calling the `PrepareAd` method. You can pass your `adUnitId` as a parameter, or call it without any parameters to use the configured `AdConfig.DefaultInterstitialAdUnitId`. After the ad is prepared, simply call `ShowAd()` to display the interstitial ad.

### Interface

```
public interface IInterstitialAdService
{
    
    bool IsAdLoaded { get; }

    IInterstitialAd CreateAd(string? adUnitId = null);

    void PrepareAd(string? adUnitId = null);

    void ShowAd();

    event EventHandler OnAdLoaded;
}
```

| Property | Description |
| --- | --- |
| `IsAdLoaded` | True when an ad is ready to be presented to the user. |

| Event | Description |
| --- | --- |
| `OnAdLoaded` | Raised when an ad is loaded, after calling PrepareAd. You can now call ShowAd to present the ad to the user. Note: This is not a catch-all event handler. When using CreateAd, you should register to the ad loaded event handler of the IInterstitialAd returned by the method. |

| Method | Description |
| --- | --- |
| `PrepareAd` | Preloads an interstitial ad using the provided `adUnitId`. If no `adUnitId` is provided, it will use configured default -  `AdConfig.DefaultInterstitialAdUnitId`. |
| `ShowAd` | Displays the interstitial ad which was prepared when `PrepareAd` was called. If no ad was prepared, nothing will be shown. |
| `CreateAd` | Creates an interstitial ad instance which you can use to preload and show later on. This enables you to preload multiple interstitial ads at the same time, using different ad unit IDs. If no `adUnitId` is provided, it will use configured default -  `AdConfig.DefaultInterstitialAdUnitId`. |

## Ad instance API (`IInterstitialAd`)

If you need full control (including per-ad event handling), use `CreateAd(...)` and subscribe to the returned `IInterstitialAd` events:

```
public interface IInterstitialAd
{
    string AdUnitId { get; }
    bool IsLoaded { get; }

    event EventHandler OnAdLoaded;
    event EventHandler<IAdError> OnAdFailedToLoad;
    event EventHandler OnAdShowed;
    event EventHandler<IAdError> OnAdFailedToShow;
    event EventHandler OnAdImpression;
    event EventHandler OnAdClicked;
    event EventHandler OnAdDismissed;

    void Load();
    void Show();
}
```

> [!TIP]
> If calling `ShowAd()` (or `Show()`) sometimes does nothing, make sure you only show the ad after it has loaded (use `IsAdLoaded` / `IsLoaded` and/or the `OnAdLoaded` event).