# BannerAd IsLoaded Property

## Overview

The `BannerAd` control includes an `IsLoaded` property that tracks the load state of the banner ad. This property allows developers to programmatically check if an ad has successfully loaded and to bind UI elements to the load state.

## Property

### IsLoaded

```csharp
public bool IsLoaded { get; }
```

A bindable read-only property that indicates whether the banner ad has successfully loaded.

- **Type**: `bool`
- **Default Value**: `false`
- **Bindable**: Yes

**Behavior:**
- Set to `false` initially and before each load attempt
- Set to `true` when the ad successfully loads (via `OnAdLoaded` event)
- Set to `false` when the ad fails to load (via `OnAdFailedToLoad` event)

## Events

### OnAdLoaded

Raised when an ad is successfully received and loaded.

```csharp
public event EventHandler OnAdLoaded
```

When this event fires, `IsLoaded` is automatically set to `true`.

### OnAdFailedToLoad

Raised when an ad request fails.

```csharp
public event EventHandler<IAdError> OnAdFailedToLoad
```

When this event fires, `IsLoaded` is automatically set to `false`.

## Usage Examples

### Example 1: Controlling Visibility with Binding

The most common use case is to show the banner only when it has successfully loaded:

```xaml
<admob:BannerAd x:Name="banner" 
                AdSize="Banner"
                AdUnitId="ca-app-pub-3940256099942544/6300978111"
                OnAdLoaded="BannerAd_OnAdLoaded"
                OnAdFailedToLoad="BannerAd_OnAdFailedToLoad"
                IsVisible="{Binding Source={x:Reference banner}, Path=IsLoaded}" />
```

In this example, the banner will be hidden until the ad successfully loads, providing a cleaner user experience.

### Example 2: Programmatic State Check

Check the load state in code:

```csharp
private void CheckBannerState()
{
    if (banner.IsLoaded)
    {
        // Ad is loaded and ready
        DisplayMessage("Banner ad is ready");
    }
    else
    {
        // Ad is not loaded yet or failed to load
        DisplayMessage("Banner ad is not loaded");
    }
}
```

### Example 3: Event Handlers with State Tracking

Handle load events and access the state:

```csharp
private void BannerAd_OnAdLoaded(object sender, EventArgs e)
{
    if (sender is BannerAd bannerAd)
    {
        Debug.WriteLine($"Banner ad loaded successfully. IsLoaded: {bannerAd.IsLoaded}");
        // You can now safely interact with the ad
    }
}

private void BannerAd_OnAdFailedToLoad(object sender, IAdError e)
{
    Debug.WriteLine($"Banner ad failed to load: {e.Message}");
    if (sender is BannerAd bannerAd)
    {
        Debug.WriteLine($"IsLoaded: {bannerAd.IsLoaded}"); // Will be false
    }
    // Implement retry logic or show alternative content
}
```

### Example 4: Conditional UI Updates

Show a loading indicator until the ad loads:

```xaml
<Grid>
    <ActivityIndicator IsRunning="{Binding Source={x:Reference banner}, Path=IsLoaded, Converter={StaticResource InvertBoolConverter}}"
                       IsVisible="{Binding Source={x:Reference banner}, Path=IsLoaded, Converter={StaticResource InvertBoolConverter}}" />
    
    <admob:BannerAd x:Name="banner"
                    AdSize="Banner"
                    OnAdLoaded="BannerAd_OnAdLoaded"
                    OnAdFailedToLoad="BannerAd_OnAdFailedToLoad"
                    IsVisible="{Binding Source={x:Reference banner}, Path=IsLoaded}" />
</Grid>
```

### Example 5: Retry Logic on Failure

Implement retry logic when ad fails to load:

```csharp
private int _retryCount = 0;
private const int MaxRetries = 3;

private void BannerAd_OnAdFailedToLoad(object sender, IAdError e)
{
    Debug.WriteLine($"Banner ad failed to load: {e.Message}");
    
    if (_retryCount < MaxRetries)
    {
        _retryCount++;
        Debug.WriteLine($"Retrying... Attempt {_retryCount}/{MaxRetries}");
        
        // Wait a bit before retrying
        Device.StartTimer(TimeSpan.FromSeconds(5), () =>
        {
            // Trigger a new load by recreating the ad or updating properties
            return false; // Don't repeat timer
        });
    }
    else
    {
        Debug.WriteLine("Max retries reached. Giving up on banner ad.");
    }
}

private void BannerAd_OnAdLoaded(object sender, EventArgs e)
{
    // Reset retry count on success
    _retryCount = 0;
    Debug.WriteLine("Banner ad loaded successfully");
}
```

## Benefits

1. **Clean UX**: Show ads only when they're ready, avoiding blank spaces or loading placeholders
2. **State Visibility**: Clear indication of whether an ad has loaded successfully
3. **Error Handling**: Easily detect and handle ad load failures
4. **Binding Support**: Leverage MAUI's data binding for declarative UI updates
5. **Consistency**: Matches the pattern used in `RewardedAd.IsLoaded` for a consistent API

## Comparison with RewardedAd

The `BannerAd.IsLoaded` property follows the same pattern as `RewardedAd.IsLoaded`:

| Feature | BannerAd | RewardedAd |
|---------|----------|------------|
| `IsLoaded` property | ✅ Yes | ✅ Yes |
| `OnAdLoaded` event | ✅ Yes | ✅ Yes |
| `OnAdFailedToLoad` event | ✅ Yes | ✅ Yes |
| Bindable property | ✅ Yes | ❌ No (regular property) |
| Auto-updates on load | ✅ Yes | ✅ Yes |
| Auto-updates on failure | ✅ Yes | ❌ No (stays in previous state) |

**Note**: `BannerAd.IsLoaded` is a `BindableProperty`, making it more suitable for XAML binding scenarios, while `RewardedAd.IsLoaded` is a regular property.

## Best Practices

1. **Always handle both events**: Implement both `OnAdLoaded` and `OnAdFailedToLoad` handlers to cover all scenarios

2. **Use binding for visibility**: Leverage `IsLoaded` binding to control visibility rather than manual updates

3. **Don't assume initial load**: Always check `IsLoaded` or wait for `OnAdLoaded` before assuming the ad is ready

4. **Implement fallbacks**: Have a plan for when ads fail to load (retry logic, alternative content, etc.)

5. **Consider layout shifts**: Use a fixed height container or placeholder to prevent layout shifts when the ad loads

## Troubleshooting

### Ad not loading (IsLoaded stays false)

1. Check that your `AdUnitId` is correct
2. Verify your app is properly configured with AdMob
3. Check consent status if using UMP (User Messaging Platform)
4. Review logcat (Android) or device logs (iOS) for AdMob error messages
5. Ensure test ads work before using production ad units

### IsLoaded changes but ad not visible

1. Check the `IsVisible` binding is correct
2. Verify the parent container is visible
3. Check for layout constraints that might hide the ad
4. Ensure the ad size is appropriate for the container

### OnAdFailedToLoad fires immediately

1. This usually indicates a configuration issue
2. Check the error message in `IAdError` for details
3. Common causes: invalid ad unit ID, no internet connection, consent not granted

## Migration Guide

If you were previously checking ad load status differently, here's how to migrate:

### Before (no load state tracking)

```xaml
<admob:BannerAd AdSize="Banner" />
```

```csharp
// No way to reliably know if ad loaded
```

### After (with IsLoaded)

```xaml
<admob:BannerAd x:Name="banner"
                AdSize="Banner"
                OnAdLoaded="BannerAd_OnAdLoaded"
                OnAdFailedToLoad="BannerAd_OnAdFailedToLoad"
                IsVisible="{Binding Source={x:Reference banner}, Path=IsLoaded}" />
```

```csharp
private void BannerAd_OnAdLoaded(object sender, EventArgs e)
{
    // Ad is ready, IsLoaded is true
}

private void BannerAd_OnAdFailedToLoad(object sender, IAdError e)
{
    // Ad failed, IsLoaded is false
}
```

## See Also

- [Banner Ads Wiki](https://github.com/marius-bughiu/Plugin.AdMob/wiki/Banner-ads)
- [Rewarded Ads Wiki](https://github.com/marius-bughiu/Plugin.AdMob/wiki/Rewarded-ads)
- [AdMob Documentation](https://developers.google.com/admob)
