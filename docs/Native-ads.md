## Displaying a native ad

Add the controls namespace at the top of your page:

```
xmlns:admob="clr-namespace:Plugin.AdMob;assembly=Plugin.AdMob"
```

and then place the ad view in your page, making sure to specify an `AdContent` which satisfies the [AdMob policies for native ads](https://support.google.com/admob/answer/6329638).

```
<admob:NativeAdView AdUnitId="ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx">
    <admob:NativeAdView.AdContent>
        <ContentView>
            <VerticalStackLayout>
                <Image Source="{Binding ImageUri}" />
                <Label Text="{Binding Headline}" FontAttributes="Bold" FontSize="18" />
                <Label Text="{Binding Body}" TextColor="Purple" />
            </VerticalStackLayout>
        </ContentView>
    </admob:NativeAdView.AdContent>
</admob:NativeAdView>
```

> [!NOTE]
> The `AdUnitId` is optional when using test ads. You can enable test ads by setting `AdConfig.UseTestAdUnitIds` to `true`.

## Displaying media content (video ads)

To display a native ad's media content (a video, or the main image when the ad has no video), place a `MediaView` inside your `AdContent`. The Google Mobile Ads SDK renders the media into it and manages playback automatically:

```
<admob:NativeAdView>
    <admob:NativeAdView.AdContent>
        <ContentView>
            <VerticalStackLayout>
                <admob:MediaView HeightRequest="200" />
                <Label Text="{Binding Headline}" FontAttributes="Bold" FontSize="18" />
                <Label Text="{Binding Body}" />
            </VerticalStackLayout>
        </ContentView>
    </admob:NativeAdView.AdContent>
</admob:NativeAdView>
```

Video playback can be configured by passing `VideoOptions` when creating the ad through the service:

```
var nativeAd = _nativeAdService.CreateAd(
    "ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx",
    new VideoOptions
    {
        StartMuted = true,
        CustomControlsRequested = false,
        ClickToExpandRequested = false,
    });
```

After the ad loads, `INativeAd` exposes the media details (`HasVideoContent`, `VideoAspectRatio`, `VideoDuration`, `VideoCurrentTime`) and raises the video lifecycle events (`OnVideoStart`, `OnVideoPlay`, `OnVideoPause`, `OnVideoEnd`, `OnVideoMuted`). Use `VideoAspectRatio` to size the `MediaView` to match the creative.

> [!NOTE]
> During development you can use Google's native **video** demo ad unit `ca-app-pub-3940256099942544/1044960115`. An explicitly specified ad unit ID always wins over `AdConfig.UseTestAdUnitIds`, which would otherwise route the request to the image-only native demo unit.

> [!NOTE]
> Some creatives report `VideoDuration` as zero at load time; the value becomes accurate once playback starts.

## Native ad model (`INativeAd`)

The binding context for `NativeAdView.AdContent` is an `INativeAd` instance. Some assets are required by policy (for example `Headline` and `Body`) and some are optional, and may be `null`.

```
public interface INativeAd
{
    string AdUnitId { get; }
    bool IsLoaded { get; }

    string? Advertiser { get; }
    string? Body { get; }
    string? CallToAction { get; }
    string? Headline { get; }
    string? IconUri { get; }
    string? ImageUri { get; }
    string? Price { get; }
    double? StarRating { get; }
    string? Store { get; }

    VideoOptions? VideoOptions { get; }
    bool HasVideoContent { get; }
    double VideoAspectRatio { get; }
    TimeSpan VideoDuration { get; }
    TimeSpan VideoCurrentTime { get; }

    event EventHandler OnAdLoaded;
    event EventHandler<IAdError> OnAdFailedToLoad;
    event EventHandler? OnAdImpression;
    event EventHandler? OnAdClicked;
    event EventHandler? OnAdSwiped;
    event EventHandler? OnAdOpened;
    event EventHandler? OnAdClosed;
    event EventHandler? OnVideoStart;
    event EventHandler? OnVideoPlay;
    event EventHandler? OnVideoPause;
    event EventHandler? OnVideoEnd;
    event EventHandler<bool>? OnVideoMuted;

    void Load();
}
```

> [!NOTE]
> `OnVideoStart` is supported only by Android. On iOS the first `OnVideoPlay` marks the beginning of playback.

### Advanced usage

For more advanced scenarios, you can prepare as many `INativeAd` instances as needed and display them using the `NativeAdView`:

```
var nativeAd = _nativeAdService.CreateAd("ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx");
nativeAd.OnAdLoaded += (_, _) =>
{
    var customAdView = new MyCustomAdTemplate();
    var nativeAdView = new NativeAdView(nativeAd, customAdView);
    this.LayoutRoot.Add(nativeAdView);
};

nativeAd.Load();
```

To obtain the service:

```
var nativeAdService = IPlatformApplication.Current.Services.GetRequiredService<INativeAdService>();
```