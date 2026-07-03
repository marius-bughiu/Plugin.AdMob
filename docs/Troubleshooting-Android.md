## Troubleshooting (Android)

## Build fails due to duplicate classes (measurement / compose / etc.)

### Symptoms

- `Type ... is defined multiple times`
- `JAVA0000: Compilation failed`
- Conflicts after adding another library (for example Firebase) or after moving to .NET 10.

### Fix / mitigation

- **Clean aggressively**:
  - Clean the solution
  - Delete `bin` and `obj`
  - Rebuild
- **If you use .NET 10 and hit duplicate Compose annotation classes** (see closed issue #67): this is fixed in .NET MAUI 10.0.80 — update `Microsoft.Maui.Controls` to 10.0.80 or later and remove the workaround. On older MAUI 10 versions, exclude the JVM variant from your app project:

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<PackageReference Include="Xamarin.AndroidX.Compose.Runtime.Annotation.Jvm" Version="1.10.0.1" ExcludeAssets="all" />
</ItemGroup>
```

- **If you use Firebase and hit duplicate Google measurement classes** (see issues #29 and #55, e.g. `Type com.google.android.gms.internal.measurement.zzbm is defined multiple times`): the Google "measurement" classes ship in both `Xamarin.GooglePlayServices.Measurement.*` (pulled in by this plugin via `Xamarin.GooglePlayServices.Ads.Lite`) and `Xamarin.Firebase.Analytics` (pulled in by Plugin.Firebase). The build only works when the whole family resolves to the same Google release train. Align them by referencing `Xamarin.Firebase.Analytics` directly in your app at the version matching the resolved `Xamarin.GooglePlayServices.Measurement.Base` (check `obj/project.assets.json`). Verified with Plugin.Firebase 4.2.1 and Xamarin.GooglePlayServices.Ads.Lite 124.0.0.6:

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<PackageReference Include="Xamarin.Firebase.Analytics" Version="123.2.0.1" />
</ItemGroup>
```

  Note: enabling MultiDex or switching to d8/r8 does **not** fix this — d8/r8 fail on duplicate type definitions rather than deduplicating them.

### Related issues

- [`#29`](https://github.com/marius-bughiu/Plugin.AdMob/issues/29) (duplicate measurement classes when combined with Firebase)
- [`#67`](https://github.com/marius-bughiu/Plugin.AdMob/issues/67) (duplicate Compose runtime annotation classes on .NET 10)

## AndroidManifest namespace warnings on .NET 10 / API 35+

### Symptoms

- Warnings like:
  - `Namespace 'com.google.android.gms.ads' is used in multiple modules and/or libraries`

### Fix / mitigation

Make sure your app explicitly sets a minimum Android platform version compatible with Google Play Services. For .NET 10, set `SupportedOSPlatformVersion` to 23:

```
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<SupportedOSPlatformVersion>23.0</SupportedOSPlatformVersion>
</PropertyGroup>
```

### Related issues

- [`#68`](https://github.com/marius-bughiu/Plugin.AdMob/issues/68) (namespace warnings when min platform is too low)

## Full-screen test ads fail with "no fill" on emulators (banners work)

### Symptoms

- Interstitial, rewarded, rewarded interstitial, and app open test ads fail to load with `Ad failed to load : 3` ("No fill.") on an Android emulator, while banner test ads load and render fine.
- Consent is granted and `AdConfig.UseTestAdUnitIds = true`, so a fill is expected every time.
- The full `LoadAdError` response shows the demo campaign *was* served, but the adapter failed instantly:

```
"Ad Source Instance Name": "[DO NOT EDIT] Publisher Test Interstitial",
"Ad Error": { "Code": 0, "Message": "Internal error.", ... },
"Latency": 0
```

### Root cause

The emulator is running with software GPU rendering (`-gpu swiftshader_indirect`, or "Graphics: Software" in the AVD settings). Full-screen ad creatives are pre-rendered at load time and fail with an internal error without hardware acceleration. The SDK surfaces this as a generic no-fill. Banner ads are not affected, which makes it look like an ad-serving or plugin problem when it is not.

### Fix / mitigation

- Start the emulator with hardware graphics acceleration: `emulator -avd <name> -gpu host`, or set **Graphics: Hardware** in the AVD settings in Android Studio.
- To see the underlying error instead of a bare "no fill", subscribe to `OnAdFailedToLoad` on the ad or ad service and log the message.
- Physical devices are unaffected.

## Native test ads fail on emulators without the Play Store

Native test ads fail with `Ad failed to load : 0` and `Incorrect native ad response. Click actions were not properly specified` on **Google APIs** emulator images: the native demo campaign serves app-install creatives whose click actions need the Play Store. Use a **Google Play** system image. See [Troubleshooting-Native-ads](Troubleshooting-Native-ads.md#native-test-ads-fail-on-android-emulators-without-the-play-store) for details.

## Runtime errors on older Android versions

### Symptoms

- `Java.Lang.NoSuchMethodError` mentioning `getDisplay()`
- Analyzer warnings like `'Context.Display' is only supported on: 'android' 30.0 and later.`

### Fix / mitigation

- Update to the latest plugin version.
- If you still reproduce it, capture the exception + target Android version and report it.

### Related issues

- [`#40`](https://github.com/marius-bughiu/Plugin.AdMob/issues/40) (older Android compatibility / `getDisplay()` errors)
