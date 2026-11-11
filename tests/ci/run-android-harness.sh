#!/usr/bin/env bash
set -euo pipefail

# Installs the pre-built device-test APK on the booted emulator, launches it, and scrapes the
# harness's result lines from logcat.
#
# Consent, the SDK start, the banner and the ads requested before consent are the hard gates. On the hosted
# runner's google_apis image the full-screen formats load too, but native/native-video do not
# (they need a Play Store image for market:// click resolution), so the full set (SUMMARY_ALL) is
# only enforced when REQUIRE_ALL=true — i.e. against a google_apis_playstore AVD on a self-hosted
# runner.

PKG="com.plugin.admob.devicetests"
TAG="AdMobHarness"
REQUIRE_ALL="${REQUIRE_ALL:-false}"
APK="${APK:?APK env var not set}"

# Dumped whenever the harness produces no usable result. Without this a startup crash is
# invisible: the tag-filtered log below is simply empty and says nothing about why.
dump_diagnostics() {
  echo "===== app process ====="
  adb shell pidof "$PKG" || echo "(app is not running)"
  echo "===== crash buffer ====="
  adb logcat -d -b crash 2>/dev/null | tail -80 || true
  echo "===== app lifecycle events ====="
  adb logcat -d -b events 2>/dev/null | grep -E "am_(proc_died|kill|crash|anr|low_memory)|$PKG" | tail -40 || true
  pid=$(adb logcat -d -s "${TAG}:I" | awk '/START/ { print $3; exit }')
  if [ -n "$pid" ]; then
    echo "===== app process $pid log ====="
    adb logcat -d -b main,system 2>/dev/null | awk -v pid="$pid" '$3 == pid' | tail -150 || true
  fi
  echo "===== last 120 log lines (unfiltered) ====="
  adb logcat -d 2>/dev/null | tail -120 || true
}

echo "Installing $APK"
adb install -r "$APK"

adb logcat -G 16M || true
adb logcat -c
echo "Launching $PKG"
adb shell monkey -p "$PKG" -c android.intent.category.LAUNCHER 1 >/dev/null

# Wait for the harness to emit its final summary line (or time out).
deadline=$((SECONDS + 600))
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

consent=$(adb logcat -d -s "${TAG}:I" | grep -F "format=consent" | tail -1 || true)
if [ -n "$consent" ] && ! echo "$consent" | grep -q "status=PASS"; then
  echo "::error::Automatic consent never completed, so no consent-gated ad can load."
  dump_diagnostics
  exit 1
fi

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

sdk=$(adb logcat -d -s "${TAG}:I" | grep -F "format=sdk-initialization " | tail -1 || true)
if [ -n "$sdk" ] && ! echo "$sdk" | grep -q "status=PASS"; then
  echo "::error::IMobileAdsService.Initialization didn't complete after consent (sdk-initialization)."
  dump_diagnostics
  exit 1
fi

for check in load-waits-for-consent prepare-before-consent; do
  line=$(adb logcat -d -s "${TAG}:I" | grep -F "format=$check " | tail -1 || true)
  if ! echo "$line" | grep -q "status=PASS"; then
    echo "::error::An ad requested before consent didn't load once consent allowed it ($check)."
    dump_diagnostics
    exit 1
  fi
done

if [ "$REQUIRE_ALL" = "true" ]; then
  if ! echo "$allline" | grep -q "status=PASS"; then
    echo "::error::Not every ad format loaded (require_all_formats=true)."
    exit 1
  fi
fi

echo "Device-test gate passed (banner and ads requested before consent loaded; require_all_formats=$REQUIRE_ALL)."
