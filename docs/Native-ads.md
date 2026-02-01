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