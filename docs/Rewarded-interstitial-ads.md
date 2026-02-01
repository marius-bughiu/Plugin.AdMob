Rewarded interstitial ads are displayed using the `IRewardedInterstitialAdService` service. The service is registered by the plugin and can be either injected or retrieved from the service provider as follows:

```
var rewardedInterstitialAdService = IPlatformApplication.Current.Services.GetService<IRewardedInterstitialAdService>();
```

Once you grab hold of the service instance, the next step is to preload the rewarded interstitial ad. You can do so by calling the `PrepareAd` method. You can pass your `adUnitId` as a parameter, or call it without any parameters to use the configured `AdConfig.DefaultRewardedInterstitialAdUnitId`. After the ad is prepared, simply call `ShowAd()` to display the rewarded interstitial ad.

### Interface

```
public interface IRewardedInterstitialAdService
{
    IRewardedInterstitialAd CreateAd(string adUnitId = null);

    void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null);

    void ShowAd();
}
```

| Property | Description |
| --- | --- |
| `IsAdLoaded` | True when an ad is ready to be presented to the user. |

| Event | Description |
| --- | --- |
| `OnAdLoaded` | Raised when an ad is loaded, after calling PrepareAd. You can now call ShowAd to present the ad to the user. Note: This is not a catch-all event handler. When using CreateAd, you should register to the ad loaded event handler of the `IRewardedInterstitialAd` returned by the method. |

| Method| Description |
| --- | --- |
| `PrepareAd` | Preloads a rewarded interstitial ad using the provided `adUnitId`. If no `adUnitId` is provided, it will use the configured default -  `AdConfig.DefaultRewardedInterstitialAdUnitId`. Use the `onUserEarnedReward` callback to get notified when the user earned his reward. |
| `ShowAd` | Displays the rewarded interstitial ad which was prepared when `PrepareAd` was called. If no ad was prepared, nothing will be shown. |
| `CreateAd` | Creates an rewarded interstitial ad instance which you can use to preload and show later on. This enables you to preload multiple rewarded interstitial ads at the same time, using different ad unit IDs. If no `adUnitId` is provided, it will use configured default -  `AdConfig.DefaultRewardedInterstitialAdUnitId`. |