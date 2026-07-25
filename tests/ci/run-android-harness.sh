#!/usr/bin/env bash
set -euo pipefail

# Installs the pre-built device-test APK on the booted emulator, launches it, and scrapes the
# harness's result lines from logcat.
#
# Banner is the hard gate. On the hosted runner's google_apis image the full-screen formats load
# too, but native/native-video do not (they need a Play Store image for market:// click
# resolution), so the full set (SUMMARY_ALL) is only enforced when REQUIRE_ALL=true — i.e. against
# a google_apis_playstore AVD on a self-hosted runner.

PKG="com.plugin.admob.devicetests"
TAG="AdMobHarness"
REQUIRE_ALL="${REQUIRE_ALL:-false}"
APK="${APK:?APK env var not set}"

# Dumped whenever the harness produces no usable result. Without this a startup crash is
# invisible: the tag-filtered log below is simply empty and says nothing about why.
dump_diagnostics() {
  echo "===== app process ====="
  adb shell pidof "$PKG" || echo "(app is not running)"
  echo "===== fatal / crash ====="
  adb logcat -d -s AndroidRuntime:E monodroid:F monodroid-assembly:F DEBUG:F 2>/dev/null | tail -40 || true
  echo "===== last 120 log lines (unfiltered) ====="
  adb logcat -d 2>/dev/null | tail -120 || true
}

echo "Installing $APK"
adb install -r "$APK"

adb logcat -c
echo "Launching $PKG"
adb shell monkey -p "$PKG" -c android.intent.category.LAUNCHER 1 >/dev/null

# Wait for the harness to emit its final summary line (or time out).
deadline=$((SECONDS + 480))
while [ "$SECONDS" -lt "$deadline" ]; do
  if adb logcat -d -s "${TAG}:I" | grep -qF "SUMMARY_ALL"; then break; fi
  sleep 5
done

echo "===== harness log ====="
adb logcat -d -s "${TAG}:I" || true
echo "======================="

if [ -n "${GITHUB_STEP_SUMMARY:-}" ]; then
  {
    echo "### Android device-test results"
    echo '```'
    adb logcat -d -s "${TAG}:I" | grep -E "RESULT|SUMMARY" || echo "(no harness output captured)"
    echo '```'
  } >> "$GITHUB_STEP_SUMMARY"
fi

banner=$(adb logcat -d -s "${TAG}:I" | grep -F "SUMMARY_BANNER" | tail -1 || true)
allline=$(adb logcat -d -s "${TAG}:I" | grep -F "SUMMARY_ALL" | tail -1 || true)

echo "banner:  ${banner:-<none>}"
echo "all:     ${allline:-<none>}"

if [ -z "$banner" ]; then
  echo "::error::Harness did not report a banner result (app crashed or never finished loading)."
  dump_diagnostics
  exit 1
fi

if ! echo "$banner" | grep -q "status=PASS"; then
  echo "::error::Banner ad failed to load against Google's test ad unit."
  dump_diagnostics
  exit 1
fi

if [ "$REQUIRE_ALL" = "true" ]; then
  if ! echo "$allline" | grep -q "status=PASS"; then
    echo "::error::Not every ad format loaded (require_all_formats=true)."
    exit 1
  fi
fi

echo "Device-test gate passed (banner loaded; require_all_formats=$REQUIRE_ALL)."
