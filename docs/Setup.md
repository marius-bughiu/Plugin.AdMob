## Initialization

In your `MauiProgram.cs` call the following method on your app builder:

```
builder.UseAdMob()
```

## Platform support

- **Android / iOS**: fully supported.
- **MacCatalyst / Windows**: the plugin registers handlers so your app can compile and run cross-platform, but ads are not displayed on these platforms.

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

Set the minimum supported Android version to **23** in your app's `.csproj` — Google Play Services requires it, and lower values cause manifest merger errors on .NET 10 (see [Troubleshooting-Android](Troubleshooting-Android.md#androidmanifest-namespace-warnings-on-net-10--api-35)):

```
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<SupportedOSPlatformVersion>23.0</SupportedOSPlatformVersion>
</PropertyGroup>
```

For more details you can check the official docs: [Get started with AdMob on Android](https://developers.google.com/admob/android/quick-start)

For a fully working example, check out the [samples folder](https://github.com/marius-bughiu/Plugin.AdMob/tree/main/samples).

## iOS setup

1. Modify your `Platforms/iOS/Info.plist` by adding your `GADApplicationIdentifier` and `SKAdNetworkItems` as described here: [Get started with AdMob on iOS](https://developers.google.com/admob/ios/quick-start)

2. Add `GADIsAdManagerApp` and `NSUserTrackingUsageDescription` (see [ATT](https://developer.apple.com/documentation/apptrackingtransparency)) to your `Platforms/iOS/Info.plist`:

```
<key>GADIsAdManagerApp</key>
<true/>
<key>NSUserTrackingUsageDescription</key>
<string>This identifier will be used to deliver personalized ads to you.</string>
```

3. Go to your `Platforms/iOS/AppDelegate.cs` and make a call to `Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null)`. Your `AppDelegate` should look similar to this:
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

4. Enable Swift library linking by adding the following to your app's `.csproj`. This is only needed starting from version 2.0.0 of the plugin.

```
<Target Name="LinkWithSwift" DependsOnTargets="_ParseBundlerArguments;_DetectSdkLocations" BeforeTargets="_LinkNativeExecutable">
    <PropertyGroup>
        <_SwiftPlatform Condition="$(RuntimeIdentifier.StartsWith('iossimulator-'))">iphonesimulator</_SwiftPlatform>
        <_SwiftPlatform Condition="$(RuntimeIdentifier.StartsWith('ios-'))">iphoneos</_SwiftPlatform>
    </PropertyGroup>
    <ItemGroup>
        <_CustomLinkFlags Include="-L" />
        <_CustomLinkFlags Include="/usr/lib/swift" />
        <_CustomLinkFlags Include="-L" />
        <_CustomLinkFlags Include="$(_SdkDevPath)/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/$(_SwiftPlatform)" />
        <_CustomLinkFlags Include="-Wl,-rpath" />
        <_CustomLinkFlags Include="-Wl,/usr/lib/swift" />
    </ItemGroup>
</Target>
```

5. Set the minimum supported iOS version to **15.0** — the Google Mobile Ads SDK requires it. Without it, `MobileAds.SharedInstance.Start(...)` can fail with a `NullReferenceException` on physical devices (see issue #56), and the `Jc.UMP.iOS` build targets fail unless `SupportedOSPlatformVersion` is set explicitly. In your app's `.csproj`:

```
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-ios'">
	<SupportedOSPlatformVersion>15.0</SupportedOSPlatformVersion>
</PropertyGroup>
```

Also add the matching key to your `Platforms/iOS/Info.plist` — Release builds fail without it:

```
<key>MinimumOSVersion</key>
<string>15.0</string>
```

For a fully working example, check out the [samples folder](https://github.com/marius-bughiu/Plugin.AdMob/tree/main/samples).