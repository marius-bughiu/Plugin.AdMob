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
- **If you use .NET 10 and hit duplicate Compose annotation classes** (see closed issue #67), exclude the JVM variant from your app project:

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
	<PackageReference Include="Xamarin.AndroidX.Compose.Runtime.Annotation.Jvm" Version="1.10.0.1" ExcludeAssets="all" />
</ItemGroup>
```

- **If you use Firebase and hit duplicate Google measurement classes** (see closed issue #29): this is usually a dependency graph conflict. After cleaning, you may need to pin or exclude the offending transitive package versions in your app.

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

## Runtime errors on older Android versions

### Symptoms

- `Java.Lang.NoSuchMethodError` mentioning `getDisplay()`
- Analyzer warnings like `'Context.Display' is only supported on: 'android' 30.0 and later.`

### Fix / mitigation

- Update to the latest plugin version.
- If you still reproduce it, capture the exception + target Android version and report it.

### Related issues

- [`#40`](https://github.com/marius-bughiu/Plugin.AdMob/issues/40) (older Android compatibility / `getDisplay()` errors)
