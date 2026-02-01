## Troubleshooting (Upgrades & target frameworks)

## “Package is not compatible with netX.Y-windows...”

### Symptoms

- NuGet restore fails with messages like:
  - `The Plugin.AdMob package is not compatible with net9.0-windows...`

### Root cause

Your app targets Windows, but the package is intended for mobile platforms. Depending on the version, the package may not include a compatible Windows target framework.

### Fix / mitigation

Conditionally reference the package only for Android/iOS target frameworks in your app `.csproj`:

```
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android' Or '$(TargetFramework)' == 'net10.0-ios'">
	<PackageReference Include="Plugin.AdMob" Version="x.y.z" />
</ItemGroup>
```

### Related issues

- [`#26`](https://github.com/marius-bughiu/Plugin.AdMob/issues/26) (upgrade failed because the app targeted Windows)

