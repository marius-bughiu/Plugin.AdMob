Rewarded ads are displayed using the `IRewardedAdService` service. The service is registered by the plugin and can be either injected or retrieved from the service provider as follows:

```
var rewardedAdService = IPlatformApplication.Current.Services.GetService<IRewardedAdService>();
```

Once you grab hold of the service instance, the next step is to preload the rewarded ad. You can do so by calling the `PrepareAd` method. You can pass your `adUnitId` as a parameter, or call it without any parameters to use the configured `AdConfig.DefaultRewardedAdUnitId`. After the ad is prepared, simply call `ShowAd()` to display the rewarded ad.

### Interface

```
public interface IRewardedAdService
{
    bool IsAdLoaded { get; }
    
    IRewardedAd CreateAd(string adUnitId = null);
    
    void PrepareAd(string adUnitId = null, Action<RewardItem> onUserEarnedReward = null);
    
    void ShowAd();
    
    event EventHandler OnAdLoaded;
}
```

| Property | Description |
| --- | --- |
| `IsAdLoaded` | True when an ad is ready to be presented to the user. |

| Event | Description |
| --- | --- |
| `OnAdLoaded` | Raised when an ad is loaded, after calling PrepareAd. You can now call ShowAd to present the ad to the user. Note: This is not a catch-all event handler. When using CreateAd, you should register to the ad loaded event handler of the `IRewardedAd` returned by the method. |

| Method| Description |
| --- | --- |
| `PrepareAd` | Preloads a rewarded ad using the provided `adUnitId`. If no `adUnitId` is provided, it will use the configured default -  `AdConfig.DefaultRewardedAdUnitId`. Use the `onUserEarnedReward` callback to get notified when the user earned his reward. |
| `ShowAd` | Displays the rewarded ad which was prepared when `PrepareAd` was called. If no ad was prepared, nothing will be shown. |
| `CreateAd` | Creates an rewarded ad instance which you can use to preload and show later on. This enables you to preload multiple rewarded ads at the same time, using different ad unit IDs. If no `adUnitId` is provided, it will use configured default -  `AdConfig.DefaultRewardedAdUnitId`. |