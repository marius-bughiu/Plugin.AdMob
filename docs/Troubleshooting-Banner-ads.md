## Troubleshooting (Banner ads)

## Banner sometimes doesn’t appear and you don’t know if it loaded

### Symptoms

- Banner shows sometimes, but not consistently.
- You want to hide the banner until it successfully loads.

### Fix / mitigation

- Use the `BannerAd` events (`OnAdLoaded`, `OnAdFailedToLoad`, etc.) and/or its `IsLoaded` property to drive UI.

See: [Banner ads](Banner-ads.md)

### Related issues

- [`#64`](https://github.com/marius-bughiu/Plugin.AdMob/issues/64) (events existed but were not documented; `IsLoaded` added for easier UX)

## Crash on iOS when specifying banner size

### Symptoms

- Setting `AdSize="LargeBanner"` (or other sizes) causes an exception on iOS.

### Fix / mitigation

- Update to the latest plugin version (this was fixed historically; see closed issue #4).

### Related issues

- [`#4`](https://github.com/marius-bughiu/Plugin.AdMob/issues/4) (iOS crash when specifying banner sizes)
