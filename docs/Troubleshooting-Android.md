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
- **If you hit duplicate Compose annotation classes on .NET 10** (issues #67 and #80, e.g. `Type androidx.compose.runtime.StableMarker is defined multiple times`): the root cause is `Xamarin.GooglePlayServices.Ads.Lite` 124.0.0.4 (shipped with plugin 3.0.2 and earlier) transitively pulling both the `.Android` and `.Jvm` variants of `Xamarin.AndroidX.Compose.Runtime.Annotation`. This is fixed in `Xamarin.GooglePlayServices.Ads.Lite` 124.0.0.5+, which plugin versions after 3.0.2 reference. On plugin 3.0.2 or earlier, either update the Ads SDK directly in your app project (verified fix):

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<PackageReference Include="Xamarin.GooglePlayServices.Ads.Lite" Version="124.0.0.6" />
</ItemGroup>
```

  or exclude the JVM variant (also verified):

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<PackageReference Include="Xamarin.AndroidX.Compose.Runtime.Annotation.Jvm" Version="1.10.0.1" ExcludeAssets="all" />
</ItemGroup>
```

  Note: updating your app's `Microsoft.Maui.Controls` version does **not** fix this — the duplicate variants come from the Ads SDK's dependency graph, not MAUI's.

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
- [`#80`](https://github.com/marius-bughiu/Plugin.AdMob/issues/80) (duplicate Compose annotation classes in Release builds on plugin 3.0.2)

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

## NU1608 warnings: "Detected package version outside of dependency constraint"

### Symptoms

Building an app that references the plugin emits warnings like:

```
warning NU1608: Detected package version outside of dependency constraint: Xamarin.AndroidX.Fragment.Ktx 1.8.8.1 requires Xamarin.AndroidX.Fragment (>= 1.8.8.1 && < 1.8.9) but version Xamarin.AndroidX.Fragment 1.8.9.1 was resolved.
```

### Root cause

Google Play Services (pulled in via `Xamarin.GooglePlayServices.Ads.Lite`) depends on older `Xamarin.AndroidX.*.Ktx` packages, whose declared upper bounds don't admit the newer AndroidX base packages that .NET MAUI resolves. The pairs are compatible in practice — NU1608 is a warning, not an error — but the noise is legitimate to want gone.

### Fix / mitigation

Reference the `Ktx` packages directly in your app project at versions aligned with the resolved base packages. Verified to produce zero NU1608 warnings with Plugin.AdMob 10.0.80-beta.7 and .NET MAUI 10.0.80 (the sample apps carry the same set):

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<PackageReference Include="Xamarin.AndroidX.Fragment.Ktx" Version="1.8.9.4" />
	<PackageReference Include="Xamarin.AndroidX.Collection.Ktx" Version="1.6.0.1" />
	<PackageReference Include="Xamarin.AndroidX.Activity.Ktx" Version="1.13.0.1" />
	<PackageReference Include="Xamarin.AndroidX.Lifecycle.ViewModel.Ktx" Version="2.11.0.1" />
	<PackageReference Include="Xamarin.AndroidX.SavedState.SavedState.Ktx" Version="1.5.0.1" />
	<PackageReference Include="Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android" Version="2.11.0.1" />
	<PackageReference Include="Xamarin.AndroidX.Lifecycle.Runtime.Ktx" Version="2.11.0.1" />
	<PackageReference Include="Xamarin.AndroidX.Lifecycle.Process" Version="2.11.0.1" />
</ItemGroup>
```

If new NU1608 warnings appear after a MAUI or plugin update, adjust the pinned versions to match the newly resolved base packages (check the warnings themselves or `obj/project.assets.json`).

### Related issues

- [`#82`](https://github.com/marius-bughiu/Plugin.AdMob/issues/82) (NU1608 dependency constraint warnings)

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
