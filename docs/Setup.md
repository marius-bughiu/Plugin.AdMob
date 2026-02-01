## Initialization

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

For a fully working example, check out the [samples folder](https://github.com/marius-bughiu/Plugin.AdMob/tree/main/samples).

## iOS setup

1. Modify your `Platforms/iOS/Info.plist` by adding your `GADApplicationIdentifier` and `SKAdNetworkItems` as described here: https://developers.google.com/admob/ios/quick-start

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

For a fully working example, check out the [samples folder](https://github.com/marius-bughiu/Plugin.AdMob/tree/main/samples).