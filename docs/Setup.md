## Initialization

In your `MauiProgram.cs` call the following method on your app builder:

```
builder.UseAdMob()
```

The plugin starts the Google Mobile Ads SDK for you, on both platforms, as soon as consent allows ad requests: consent stored from an earlier session counts as soon as the session's consent request starts. If you disabled the consent check, it starts the SDK right before your first ad request. Google asks apps not to start the SDK before consent is gathered, so you don't need to call `MobileAds.SharedInstance.Start(...)` (iOS) or `MobileAds.Initialize(...)` (Android) yourself. If you do (for example to wait for mediation adapters), call it only once `IAdConsentService.CanRequestAds()` is true.

## Platform support

- **Android / iOS**: fully supported.
- **MacCatalyst / Windows**: the plugin registers handlers so your app can compile and run cross-platform, but ads are not displayed on these platforms.

## Android setup

You need to update your app's manifest (`Platforms/Android/AndroidManifest.xml`) to include:
- the `APPLICATION_ID` (make sure this is added inside the `<application>` element)
- the `ACCESS_NETWORK_STATE` and `INTERNET` permissions (if they are not present already)

```
<?xml version="1.0" encoding="utf-8"?>
<manifest ...>
  <application ...>
    <meta-data
    	android:name="com.google.android.gms.ads.APPLICATION_ID"
    	android:value="ca-app-pub-3940256099942544~3347511713" />
  </application>
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  <uses-permission android:name="android.permission.INTERNET" />
</manifest>
```

The Google Mobile Ads SDK declares the activity it shows full-screen ads in, so your manifest doesn't need one. If your manifest has a `com.google.android.gms.ads.AdActivity` entry from an earlier setup, remove it: the Next-Gen SDK doesn't have that class.

Set the minimum supported Android version to **24** in your app's `.csproj` (23 is enough on Plugin.AdMob 10.0.120.2 and earlier) — the Google Mobile Ads Next-Gen SDK depends on `androidx.webkit`, which requires API 24, and lower values cause manifest merger errors (see [Troubleshooting-Android](Troubleshooting-Android.md#androidmanifest-namespace-warnings-on-net-10--api-35)):

```
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<SupportedOSPlatformVersion>24.0</SupportedOSPlatformVersion>
</PropertyGroup>
```

For more details you can check the official docs: [Get started with the GMA Next-Gen SDK on Android](https://developers.google.com/admob/android/next-gen/quick-start)

For a fully working example, check out the [samples folder](https://github.com/marius-bughiu/Plugin.AdMob/tree/main/samples).

## iOS setup

1. Modify your `Platforms/iOS/Info.plist` by adding your `GADApplicationIdentifier` and `SKAdNetworkItems` as described here: [Get started with AdMob on iOS](https://developers.google.com/admob/ios/quick-start)

2. Add `GADIsAdManagerApp` to your `Platforms/iOS/Info.plist`. If your app requests App Tracking Transparency (ATT) permission, including through an AdMob IDFA message, also add `NSUserTrackingUsageDescription` with a clear description of why your app requests tracking:

```
<key>GADIsAdManagerApp</key>
<true/>
<key>NSUserTrackingUsageDescription</key>
<string>This identifier will be used to deliver personalized ads to you.</string>
```

See [Consent](Consent.md#ios-ump-idfa-and-att) for the recommended UMP and ATT message flow. When using an AdMob IDFA message, let UMP present the explainer and native ATT prompt; don't request ATT separately in app code.

> [!NOTE]
> Google also recommends adding [App Transport Security exceptions](https://developers.google.com/admob/ios/app-transport-security) to your `Info.plist`, as production ad creatives may serve media over non-HTTPS connections.

3. Enable Swift library linking by adding the following to your app's `.csproj`. This is only needed starting from version 2.0.0 of the plugin.

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

4. Set the minimum supported iOS version to **15.0** — the Google Mobile Ads SDK requires it. Without it, starting the SDK can fail with a `NullReferenceException` on physical devices (see issue #56), and the `Jc.UMP.iOS` build targets fail unless `SupportedOSPlatformVersion` is set explicitly. In your app's `.csproj`:

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
