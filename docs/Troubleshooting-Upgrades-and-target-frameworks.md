## Troubleshooting (Upgrades & target frameworks)

For what to change when you update the plugin, for example 10.0.120.3's Android API 24 minimum, `Xamarin.AndroidX.Fragment.Ktx` 1.9.1 requirement and legacy SDK conflicts, see [Upgrading](Upgrading.md).

## “Package is not compatible with netX.Y-windows...”

### Symptoms

- NuGet restore fails with messages like:
  - `The Plugin.AdMob package is not compatible with net9.0-windows...`

### Root cause

Your app targets Windows, but the package is intended for mobile platforms. Depending on the version, the package may not include a compatible Windows target framework.

### Fix / mitigation

Conditionally reference the package only for Android/iOS target frameworks in your app `.csproj`:

```
<ItemGroup Condition="'$(TargetFramework)' == 'net11.0-android' Or '$(TargetFramework)' == 'net11.0-ios'">
	<PackageReference Include="Plugin.AdMob" Version="x.y.z" />
</ItemGroup>
```

### Related issues

- [`#26`](https://github.com/marius-bughiu/Plugin.AdMob/issues/26) (upgrade failed because the app targeted Windows)

