# Google AdMob plugin for .NET MAUI

[![NuGet version (Plugin.AdMob)](https://img.shields.io/nuget/v/Plugin.AdMob.svg?style=flat-square)](https://www.nuget.org/packages/Plugin.AdMob/) [![NuGet pre-release (Plugin.AdMob)](https://img.shields.io/nuget/vpre/Plugin.AdMob.svg?style=flat-square)](https://www.nuget.org/packages/Plugin.AdMob/) [![NuGet downloads (Plugin.AdMob)](https://img.shields.io/nuget/dt/Plugin.AdMob.svg?style=flat-square)](https://www.nuget.org/packages/Plugin.AdMob/) [![MIT license](https://img.shields.io/github/license/marius-bughiu/Plugin.AdMob?style=flat-square)](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/LICENSE)

Monetize your .NET MAUI app with Google AdMob ads on Android and iOS — banner, interstitial, rewarded, rewarded interstitial, app open, and native ads (including native video) — with GDPR consent handled for you via Google's User Messaging Platform (UMP). Free and open source, with every feature included.

```bash
dotnet add package Plugin.AdMob
```

## Features

|         | Banner | Interstitial | Rewarded | Rewarded interstitial | App open | Native | Native video | Consent (UMP) |
| ------- | ------ | ------------ | -------- | --------------------- | -------- | ------ | ------------ | ------------- |
| Android | ✅    | ✅           | ✅      | ✅                    | ✅      | ✅    | ✅           | ✅            |
| iOS     | ✅    | ✅           | ✅      | ✅                    | ✅      | ✅    | ✅           | ✅            |

## Why Plugin.AdMob?

- **Every AdMob ad format** — banners in all sizes (including custom), full-screen interstitial, rewarded, and rewarded interstitial ads, app open ads, and fully customizable native ads with video playback.
- **Free and open source** — MIT-licensed, no paid tiers, no locked features.
- **Consent built in** — the Google UMP consent form (GDPR) is shown automatically on startup when required, or take full control yourself via `IAdConsentService`.
- **MAUI-first API** — drop a `BannerAd` or `NativeAdView` into your XAML, and use dependency-injected services (`IInterstitialAdService`, `IRewardedAdService`, `IAppOpenAdService`, ...) for full-screen ads.
- **Test ads out of the box** — set `AdConfig.UseTestAdUnitIds = true` and develop safely against Google's test ad units.
- **Cross-platform friendly** — your app still compiles and runs on Windows and Mac Catalyst; ads are simply not shown there.
- **Actively maintained** — built for .NET 10 and the latest .NET MAUI, on top of up-to-date Google Mobile Ads SDK bindings.

## Quick start

**1. Install the package:**

```bash
dotnet add package Plugin.AdMob
```

**2. Initialize the plugin** in your `MauiProgram.cs`:

```csharp
builder
    .UseMauiApp<App>()
    .UseAdMob();
```

Then complete the one-time platform configuration (Android manifest, iOS `Info.plist`) described in the [Setup guide](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Setup.md).

**3. Show a banner ad** by dropping the control into any page:

```xml
<ContentPage xmlns:admob="clr-namespace:Plugin.AdMob;assembly=Plugin.AdMob"
             ...>

    <admob:BannerAd AdUnitId="ca-app-pub-xxxxxxxxxxxxxxxx/xxxxxxxxxx" />

</ContentPage>
```

**4. Show an interstitial ad:**

```csharp
var interstitialAdService = IPlatformApplication.Current.Services.GetRequiredService<IInterstitialAdService>();
interstitialAdService.PrepareAd();
// Once loaded (OnAdLoaded), e.g. between levels:
interstitialAdService.ShowAd();
```

**Tip:** no ad unit IDs yet? Set `AdConfig.UseTestAdUnitIds = true` to use Google's test ads while developing.

### 📽️ Video tutorial

[![How to display an AdMob banner ad in your .NET MAUI app in under 5 minutes](https://i.ytimg.com/vi/-oxEb6aCAKQ/hqdefault.jpg?sqp=-oaymwEnCNACELwBSFryq4qpAxkIARUAAIhCGAHYAQHiAQoIGBACGAY4AUAB&rs=AOn4CLBLqbS3LegqEUbJVp_A3DE68ufjVw)](https://www.youtube.com/watch?v=-oxEb6aCAKQ "How to display an AdMob banner ad in your .NET MAUI app in under 5 minutes")

## Documentation

1. [Setup](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Setup.md) — initialization, Android manifest and iOS `Info.plist` configuration
1. [Configuration](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Configuration.md) — default ad unit IDs, test ads, test devices
1. [Consent](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Consent.md) — GDPR consent with Google UMP, debug geographies
1. [Banner ads](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Banner-ads.md) — all banner sizes, custom sizes, events
1. [Interstitial ads](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Interstitial-ads.md) — preload and show full-screen ads
1. [Rewarded ads](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Rewarded-ads.md) — reward users for watching ads
1. [Rewarded interstitial ads](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Rewarded-interstitial-ads.md)
1. [Native ads](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Native-ads.md) — design your own ad layout, including video
1. [App open ads](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/App-open-ads.md) — ads on app launch and foregrounding
1. [Troubleshooting](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Troubleshooting.md)
1. [Samples](https://github.com/marius-bughiu/Plugin.AdMob/blob/main/docs/Samples.md)

## Contributing & support

Found a bug or missing a feature? [Open an issue](https://github.com/marius-bughiu/Plugin.AdMob/issues) — pull requests are welcome.

If Plugin.AdMob helps your app earn money, consider giving the repo a ⭐ — it helps other .NET MAUI developers discover it.

---

_This project has no affiliation with Google, Microsoft, or the MAUI/Xamarin teams. AdMob is a trademark of Google LLC._
