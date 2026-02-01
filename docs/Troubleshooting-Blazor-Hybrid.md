## Troubleshooting (Blazor Hybrid)

## iOS: “The current view controller cannot be detected.”

### Symptoms

- The app crashes on startup when a `BannerAd` is present in the initial page.
- Removing the `BannerAd` makes the app run.

### Fix / mitigation

- Ensure you are using the latest version of the plugin.
- If your app structure still initializes UI in a way that delays view controller availability, delay ad creation:
  - show a splash/landing page first, then navigate to the page containing `BannerAd`, or
  - add the `BannerAd` after the page has appeared.

### Related issues

- [`#35`](https://github.com/marius-bughiu/Plugin.AdMob/issues/35) (Blazor Hybrid iOS view controller detection)

