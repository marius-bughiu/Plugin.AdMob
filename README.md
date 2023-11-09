# AdMob plugin for .NET MAUI

[![NuGet version (Plugin.AdMob)](https://img.shields.io/nuget/v/Plugin.AdMob.svg?style=flat-square)](https://www.nuget.org/packages/Plugin.AdMob/)

*This project has no affiliation with Microsoft or the Maui/Xamarin teams.*

* This is still in development (and the packages in preview). It is very likely that there will be breaking changes before the first release.
* Requires the latest Visual Studio 2022 Preview. 

## Initializing the plugin

In your `MauiProgram.cs` call the following method on your app builder:

```
builder.UseAdMob()
```

## Android setup

You need to update your app's manifest (`Platforms/Android/AndroidManifest.xml`) to include:
- the `AdActivity` activity
- the `ACCESS_NETWORK_STATE` and `INTERNET` permissions (if they are not present already)
- the `APPLICATION_ID` (make sure this is added inside the `<application>` element)

```
<?xml version="1.0" encoding="utf-8"?>
<manifest ...>
  <application ...>
    <activity android:name="com.google.android.gms.ads.AdActivity" android:configChanges="keyboard|keyboardHidden|orientation|screenLayout|uiMode|screenSize|smallestScreenSize" android:theme="@android:style/Theme.Translucent" />
    <meta-data
    	android:name="com.google.android.gms.ads.APPLICATION_ID"
    	android:value="ca-app-pub-3940256099942544~3347511713" />
  </application>
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  <uses-permission android:name="android.permission.INTERNET" />
</manifest>
```
For more details you can check the official docs: https://developers.google.com/admob/android/quick-start

## iOS setup

1. Modify your `Platforms/iOS/Info.plist` by adding your `GADApplicationIdentifier` and `SKAdNetworkItems` as described here: https://developers.google.com/admob/ios/quick-start
2. Go to your `Platforms/iOS/AppDelegate.cs` and make a call to `Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null)`. Your `AppDelegate` should look similar to this:
```
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp();

        Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);

        return app;
    }
}
```

## Configuration

Each individual ad view allows you to specify an `AdUnitId`. 
You can also define default ad unit IDs in case you want to use the same IDs all throughout the application. You can set them through `AdConfig.DefaultBannerAdUnitId` and `AdConfig.DefaultInterstitialAdUnitId`.

For testing purposes, you can set `AdConfig.UseTestAdUnitIds`. This will override any ad unit IDs you may have defined in your app, with AdMob test IDs which display test ads and are safe to interact with.

## Displaying a banner ad

Add the controls namespace at the top of your page:

```
xmlns:admob="clr-namespace:Plugin.AdMob;assembly=Plugin.AdMob"
```

and then place the banner in your page. *Note: the `AdUnitId` is optional when using test ads - `AdConfig.UseTestAdUnitIds = true`.*

```
<admob:BannerAd AdUnitId="ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx" />
```

# Samples

Check out the [samples](https://github.com/marius-bughiu/Plugin.AdMob/tree/main/samples/Samples) folder for fully functional plugin integration examples.

![image](https://github.com/marius-bughiu/Plugin.AdMob/assets/11870708/f95ad204-8366-4a6d-81e8-2c46bf06cd2c)
![image](https://github.com/marius-bughiu/Plugin.AdMob/assets/11870708/50bda781-0d5a-43b4-9ee8-ecc9fc62444a)

