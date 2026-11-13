# Integration tests & CI gates

Two tiers of automated checks, both aimed at the same failure class: the plugin wraps
third-party native bindings (`Xamarin.Google.Android.Libraries.Ads.Mobile.Sdk`, `Jc.GMA.iOS`,
`Jc.UMP.iOS`, `Xamarin.AndroidX.*.Ktx`) that drift independently of this repo, and the
plugin version tracks MAUI — so "the world changed under a shipped release" is a real,
time-driven risk that source-only CI can't catch.

## Projects

| Project | What it is | Used by |
|---|---|---|
| [`Plugin.AdMob.PackageConsumer`](Plugin.AdMob.PackageConsumer/) | A MAUI **library** that references the **published** NuGet across all 4 target frameworks. Pure restore + build smoke. | `nightly.yml` |
| [`Plugin.AdMob.DeviceTests`](Plugin.AdMob.DeviceTests/) | A MAUI **app** that runs the plugin's default consent flow, then loads every ad format with Google's test units and asserts `OnAdLoaded` fires, logging results under the `AdMobHarness` tag. | `device-tests.yml` |

Both switch between referencing the **source** and the **published package** via MSBuild
properties, so the same code validates a branch change *and* the shipped artifact:

```bash
# source (default) — validates the code on this branch
dotnet build tests/Plugin.AdMob.DeviceTests/Plugin.AdMob.DeviceTests.csproj -f net11.0-android

# published package — validates the shipped .nupkg
dotnet build tests/Plugin.AdMob.DeviceTests/Plugin.AdMob.DeviceTests.csproj -f net11.0-android \
  -p:UsePublishedAdMob=true -p:AdMobPackageVersion=11.0.0.0 -p:MauiVersion=11.0.0
```

## Workflows

- **`build.yml`** — source-build gate on every PR and every push to `main` or a `support/*`
  branch. Restores + compiles the plugin and sample app across Android / iOS / MacCatalyst /
  Windows. Hard-fails on **NU1605** (package downgrade); surfaces **NU1608** (native-binding
  constraint drift — the issue #68 / #82 class) to the run summary.
- **`device-tests.yml`** — runs the harness on an Android emulator and an iOS simulator, on every
  PR and every push to `main` or a `support/*` branch (docs-only changes skipped). Both lanes
  gate on consent. Android also gates on the SDK start (`sdk-initialization`),
  the banner and the ads requested before consent (`load-waits-for-consent`,
  `prepare-before-consent`); iOS reports their fill and fails only if
  one loads before consent. Reusable by the nightly and the bump gate.
- **`nightly.yml`** — 04:00 UTC. Resolves the newest published stable version of the branch's
  major (from `MauiPackageVersion`) and runs both the build smoke and the device tests **against
  the published package**. Until that major has a stable release, it skips both with a notice.
- **`bump-maui.yml`** — the MAUI auto-bump now *validates before it publishes*. It pushes the
  candidate to a throwaway `bump-maui-validate` branch, runs `build.yml` + `device-tests.yml`
  against that commit, and only then fast-forwards the reviewed `bump-maui-version` branch and
  opens/updates the PR. So the head of an open bump PR is never replaced by an unvalidated
  commit, and its body always describes the commit actually on the branch. If someone pushes a
  manual commit onto `bump-maui-version`, the promotion is skipped rather than clobbering it.
- **`release.yml`** — runs when a release tag (`v<MauiPackageVersion>.<build>`, optionally with a
  prerelease suffix such as `v10.0.120.1-beta.1`) is pushed, or manually from such a tag. It fails
  without the tag, checks the package version and that all four target frameworks are in the
  package, publishes to nuget.org through trusted publishing (no API key secret) and creates the
  GitHub Release. A stable release becomes the latest GitHub release only if no higher version
  has been released, so a release from a support branch doesn't displace a newer major.
- **`binding-drift.yml`** — weekly. Keeps a "Native binding updates available" issue open while a
  package pinned in the plugin's csproj (the Google ad SDK bindings, MinVer) is behind nuget.org.
- **`dependabot.yml`** — weekly GitHub Actions updates. NuGet isn't tracked there, because its
  dependency submission can't restore MAUI projects without the workloads.

## The banner-vs-full-screen gating model (important)

Not every format fills in every environment, and that is an environment property rather than a
plugin bug. Measured on the GitHub-hosted runner (headless, software GPU, `google_apis` image):

| Format | Headless CI (`google_apis`) | Local `-gpu host` + Play Store image |
|---|---|---|
| banner | ✅ | ✅ |
| interstitial / rewarded / rewarded-interstitial / app-open | ✅ | ✅ |
| native, native-video | ❌ `Internal error` | ✅ |

So the full-screen formats *do* load headless. The one real gap is **native**, which serves
app-install creatives whose click actions need `market://` resolution — that requires a
**Play Store** system image (`google_apis_playstore`), not merely a GPU. Loads are also
occasionally flaky (`Internal error` on a first attempt), which is why each format gets a retry.

The harness uses the plugin's defaults (`UseAdMob()`: automatic consent, consent check on), with
the `Other` debug geography so UMP resolves without showing a form. The first result line,
`format=consent`, passes once UMP reports `CanRequestAds`. If the automatic consent hook never
fires (as happened on iOS scene-based apps before 10.0.120.0), the banner can't load either, so the
banner gate also covers consent. The iOS harness is scene-based (`SceneDelegate` plus
`UIApplicationSceneManifest`), because apps built with the iOS 27 SDK don't launch without scenes.

So the harness emits two summary lines and CI gates accordingly:

- `SUMMARY_BANNER` → **hard gate** on Android; reported on iOS.
- `SUMMARY_ALL` → every format; only hard-gated when `require_all_formats: true`.

To get **full-format** coverage, run `device-tests.yml` (or the harness directly) against a
**Play Store (`google_apis_playstore`) AVD on a self-hosted runner** and pass
`require_all_formats: true`. The maintainer's local `plugin_admob_ps` AVD is exactly that.

Run the harness locally against a booted emulator:

```bash
dotnet build tests/Plugin.AdMob.DeviceTests/Plugin.AdMob.DeviceTests.csproj -c Debug \
  -p:TargetFrameworks=net11.0-android -p:EmbedAssembliesIntoApk=true \
  "-p:AndroidSdkDirectory=%LOCALAPPDATA%\Android\Sdk" "-p:JavaSdkDirectory=%LOCALAPPDATA%\Android\Jdk"
adb install -r tests/Plugin.AdMob.DeviceTests/bin/Debug/net11.0-android/com.plugin.admob.devicetests-Signed.apk
adb logcat -c && adb shell monkey -p com.plugin.admob.devicetests -c android.intent.category.LAUNCHER 1
adb logcat -s AdMobHarness:I   # watch RESULT / SUMMARY_* lines
```

`EmbedAssembliesIntoApk=true` is **required** whenever you install with `adb install`. Without
it a Debug build uses Fast Deployment, leaves the managed assemblies out of the APK, and the app
aborts at startup with `No assemblies found in .../.__override__/...` before logging anything.

### The iOS leg

The iOS simulator job runs on the `xcode-27` image (preview). .NET for iOS requires the Xcode its
workload was built with (27.2 for .NET 11.0.100), which `macos-latest` doesn't have, so from July to October 2026 the job
failed on every run while `continue-on-error` kept the workflow green. It now gates on the app
starting, consent completing and no ad loading before consent. Fill is reported, not gated, because
Google's test ads sometimes time out on the iOS simulators, and GMA 12 reports some timeouts as
`Invalid request`.

## Required checks

The `main protect` ruleset requires the four `build.yml` checks (`net11.0-android`,
`net11.0-ios`, `net11.0-maccatalyst`, `net11.0-windows10.0.19041.0`); `support protect` keeps
the `net10.0-*` names for `support/net10`. The device tests run on
every PR but aren't required, so a fill hiccup in Google's test ads can't block a merge; read
their result before merging.

PRs opened by `bump-maui.yml` use the default `GITHUB_TOKEN`. Since 2026-06-11 their
`pull_request` runs wait for a maintainer to click **Approve workflows to run**, and the request
expires after 30 days. The bump validates inline before opening the PR and asks the repository
owner for a review. To let the checks run on bump PRs without the click, give the workflow a
GitHub App installation token instead of `GITHUB_TOKEN`.

## Support branches

`support/net10` starts at the v10.0.120.3 release and carries .NET 10 fixes and MAUI 10 bumps
while .NET MAUI 10 is supported, after `main` moves to .NET 11. PRs into it and pushes to it run
`build.yml` and `device-tests.yml` from the branch's own copies, and its release tags publish
10.x packages through its own `release.yml`. Schedules only run on `main`, so the `support-net10`
jobs in `main`'s `nightly.yml` and `bump-maui.yml` call the branch's copies of those workflows
with `ref: support/net10`. Its nightly tests the newest published 10.x package, and a MAUI 10 bump
is validated on `bump-maui-support-net10-validate` and proposed from `bump-maui-support-net10`
into `support/net10`. A broken copy on the branch fails `main`'s scheduled run as well, and the
jobs go when the branch is retired. `binding-drift.yml` runs for `main` only.
