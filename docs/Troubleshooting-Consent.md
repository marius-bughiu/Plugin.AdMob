## Troubleshooting (Consent / UMP / GDPR)

## Ads never load / native ads not showing, but banners might

### Symptoms

- Ads don’t show, especially on a real device.
- You’re in a geography where consent may be required.

### Root cause

When consent is required, the plugin will not request ads until consent requirements are satisfied (unless you intentionally disabled consent checks).

### Fix / mitigation

- Ensure you configured consent dialogs in AdMob’s **Privacy & messaging** section.
- Use `IAdConsentService` to manage the consent flow if you opted out of automatic consent handling.
- During testing, force a geography and reset consent using debug settings (only works on test devices):

```
builder
    .UseAdMob()
    .UseConsentDebugSettings(new ConsentDebugSettings
    {
        Geography = ConsentDebugGeography.Eea,
        TestDeviceHashedIds = ["<hashed-device-id>"],
        Reset = true
    });
```

> [!IMPORTANT]
> You can disable consent checks via `builder.UseAdMob(disableConsentCheck: true, ...)` or `AdConfig.DisableConsentCheck = true`, but compliance becomes your responsibility.

### Related issues

- [`#5`](https://github.com/marius-bughiu/Plugin.AdMob/issues/5) / [`#9`](https://github.com/marius-bughiu/Plugin.AdMob/issues/9) (consent / GDPR / UMP requests)
- [`#66`](https://github.com/marius-bughiu/Plugin.AdMob/issues/66) (native ads gated by consent; resolved by using consent debug settings during testing)
