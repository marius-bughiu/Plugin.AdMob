# Integration tests & CI gates

Two tiers of automated checks, both aimed at the same failure class: the plugin wraps
third-party native bindings (`Xamarin.GooglePlayServices.Ads.Lite`, `Jc.GMA.iOS`,
`Jc.UMP.iOS`, `Xamarin.AndroidX.*.Ktx`) that drift independently of this repo, and the
plugin version tracks MAUI — so "the world changed under a shipped release" is a real,
time-driven risk that source-only CI can't catch.

## Projects

| Project | What it is | Used by |
|---|---|---|
| [`Plugin.AdMob.PackageConsumer`](Plugin.AdMob.PackageConsumer/) | A MAUI **library** that references the **published** NuGet across all 4 target frameworks. Pure restore + build smoke. | `nightly.yml` |
| [`Plugin.AdMob.DeviceTests`](Plugin.AdMob.DeviceTests/) | A MAUI **app** that loads every ad format with Google's test units and asserts `OnAdLoaded` fires, logging results under the `AdMobHarness` tag. | `device-tests.yml` |

Both switch between referencing the **source** and the **published package** via MSBuild
properties, so the same code validates a branch change *and* the shipped artifact:

```bash
# source (default) — validates the code on this branch
dotnet build tests/Plugin.AdMob.DeviceTests/Plugin.AdMob.DeviceTests.csproj -f net10.0-android

# published package — validates the shipped .nupkg
dotnet build tests/Plugin.AdMob.DeviceTests/Plugin.AdMob.DeviceTests.csproj -f net10.0-android \
  -p:UsePublishedAdMob=true -p:AdMobPackageVersion=10.0.90 -p:MauiVersion=10.0.90
```

## Workflows

- **`build.yml`** — source-build gate on every PR / push to `main`. Restores + compiles the
  plugin and sample app across Android / iOS / MacCatalyst / Windows. Hard-fails on **NU1605**
  (package downgrade); surfaces **NU1608** (native-binding constraint drift — the issue #68 /
  #82 class) to the run summary.
- **`device-tests.yml`** — runs the harness on an Android emulator (gating) and, best-effort,
  an iOS simulator (advisory). Reusable by the nightly and the bump gate.
- **`nightly.yml`** — 04:00 UTC. Resolves the latest published version and runs both the build
  smoke and the device tests **against the published package**.
- **`bump-maui.yml`** — the MAUI auto-bump now *validates before it publishes*. It pushes the
  candidate to a throwaway `bump-maui-validate` branch, runs `build.yml` + `device-tests.yml`
  against that commit, and only then fast-forwards the reviewed `bump-maui-version` branch and
  opens/updates the PR. So the head of an open bump PR is never replaced by an unvalidated
  commit, and its body always describes the commit actually on the branch. If someone pushes a
  manual commit onto `bump-maui-version`, the promotion is skipped rather than clobbering it.

## The banner-vs-full-screen gating model (important)

On a **headless, software-GPU emulator** — which is every GitHub-hosted runner, since they have
no GPU — only the **banner** format reliably fills. Full-screen (interstitial / rewarded /
rewarded-interstitial / app-open) and native creatives need a **GPU-backed emulator (`-gpu host`)**
to pre-render, and otherwise come back as no-fill. This is a documented, hardware-observed quirk,
not a plugin bug.

So the harness emits two summary lines and CI gates accordingly:

- `SUMMARY_BANNER` → **hard gate** on any runner.
- `SUMMARY_ALL` → every format; only hard-gated when `require_all_formats: true`.

To get **full-format** coverage, run `device-tests.yml` (or the harness directly) against a
**`-gpu host` emulator on a self-hosted runner** and pass `require_all_formats: true`. The
maintainer's local `plugin_admob_ps` (Play Store) AVD is exactly such an environment.

Run the harness locally against a booted `-gpu host` emulator:

```bash
dotnet build tests/Plugin.AdMob.DeviceTests/Plugin.AdMob.DeviceTests.csproj -c Debug -f net10.0-android \
  "-p:AndroidSdkDirectory=%LOCALAPPDATA%\Android\Sdk" "-p:JavaSdkDirectory=%LOCALAPPDATA%\Android\Jdk"
adb install -r tests/Plugin.AdMob.DeviceTests/bin/Debug/net10.0-android/com.plugin.admob.devicetests-Signed.apk
adb logcat -c && adb shell monkey -p com.plugin.admob.devicetests -c android.intent.category.LAUNCHER 1
adb logcat -s AdMobHarness:I   # watch RESULT / SUMMARY_* lines
```

## One manual step: make the gates required

Workflows run automatically, but marking them **required status checks** is a repository
setting (Settings → Branches → branch protection for `main`): add **`build`** (and, if you want
it blocking, the device-tests **`android`** job) as required.

Note: PRs opened by `bump-maui.yml` use the default `GITHUB_TOKEN`, which by design does not
trigger further workflow runs — that is why the bump validates inline before opening the PR. If
you want the checks to also render on the PR itself, have the bump use a PAT instead.
