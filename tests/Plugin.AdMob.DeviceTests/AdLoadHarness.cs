using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.DeviceTests;

/// <summary>
/// Drives a load of every ad format using Google's test ad units and reports whether each
/// fired its OnAdLoaded callback. Asserting on LOAD (a network + SDK callback) rather than
/// on-screen rendering is what makes this a stable signal for catching upstream binding /
/// MAUI drift in the published package.
///
/// Environment note baked in from the maintainer's own testing: on a headless, software-GPU
/// emulator (GitHub-hosted runners have no GPU) only the BANNER format reliably fills. The
/// full-screen and native formats need a GPU-backed emulator (-gpu host) to pre-render their
/// creative and otherwise come back as no-fill. Hence two summary lines are emitted:
///   SUMMARY_BANNER — the format CI can hard-gate on any runner.
///   SUMMARY_ALL    — every format; only meaningful (hard-gate-able) on a -gpu host runner.
/// </summary>
internal static class AdLoadHarness
{
    private const int TimeoutSeconds = 30;
    private const int Attempts = 2;

    public static async Task RunAllAsync(Layout bannerHost)
    {
        HarnessLog.Line("START");

        // Give the Mobile Ads SDK a moment to initialize before the first request.
        await Task.Delay(TimeSpan.FromSeconds(3));

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("No MAUI service provider available.");

        var interstitial = services.GetRequiredService<IInterstitialAdService>();
        var rewarded = services.GetRequiredService<IRewardedAdService>();
        var rewardedInterstitial = services.GetRequiredService<IRewardedInterstitialAdService>();
        var appOpen = services.GetRequiredService<IAppOpenAdService>();
        var native = services.GetRequiredService<INativeAdService>();

        // Banner first — the one format that fills on a headless software-GPU emulator.
        var banner = await RunBannerAsync(bannerHost);

        var results = new List<bool>
        {
            banner,
            await RunInterstitialAsync(interstitial.CreateAd()),
            await RunRewardedAsync(rewarded.CreateAd()),
            await RunRewardedInterstitialAsync(rewardedInterstitial.CreateAd()),
            await RunAppOpenAsync(appOpen.CreateAd()),
            await RunNativeAsync("native", native.CreateAd()),
            await RunNativeAsync("native-video", native.CreateAd(null, new VideoOptions { StartMuted = true })),
        };

        var passed = results.Count(x => x);

        HarnessLog.Line($"SUMMARY_BANNER status={(banner ? "PASS" : "FAIL")}");
        HarnessLog.Line($"SUMMARY_ALL status={(passed == results.Count ? "PASS" : "FAIL")} passed={passed} total={results.Count}");
    }

    private static Task<bool> RunInterstitialAsync(IInterstitialAd ad) => AwaitLoadAsync(
        "interstitial",
        (l, f) => { ad.OnAdLoaded += l; ad.OnAdFailedToLoad += f; },
        (l, f) => { ad.OnAdLoaded -= l; ad.OnAdFailedToLoad -= f; },
        ad.Load);

    private static Task<bool> RunRewardedAsync(IRewardedAd ad) => AwaitLoadAsync(
        "rewarded",
        (l, f) => { ad.OnAdLoaded += l; ad.OnAdFailedToLoad += f; },
        (l, f) => { ad.OnAdLoaded -= l; ad.OnAdFailedToLoad -= f; },
        ad.Load);

    private static Task<bool> RunRewardedInterstitialAsync(IRewardedInterstitialAd ad) => AwaitLoadAsync(
        "rewarded-interstitial",
        (l, f) => { ad.OnAdLoaded += l; ad.OnAdFailedToLoad += f; },
        (l, f) => { ad.OnAdLoaded -= l; ad.OnAdFailedToLoad -= f; },
        ad.Load);

    private static Task<bool> RunAppOpenAsync(IAppOpenAd ad) => AwaitLoadAsync(
        "app-open",
        (l, f) => { ad.OnAdLoaded += l; ad.OnAdFailedToLoad += f; },
        (l, f) => { ad.OnAdLoaded -= l; ad.OnAdFailedToLoad -= f; },
        ad.Load);

    private static Task<bool> RunNativeAsync(string format, INativeAd ad) => AwaitLoadAsync(
        format,
        (l, f) => { ad.OnAdLoaded += l; ad.OnAdFailedToLoad += f; },
        (l, f) => { ad.OnAdLoaded -= l; ad.OnAdFailedToLoad -= f; },
        ad.Load);

    /// <summary>
    /// Subscribes to a format's load / fail events, triggers the load on the UI thread, and
    /// waits (with a timeout and a retry) for the first callback. The subscribe/unsubscribe
    /// lambdas capture the concrete ad instance so no shared base interface is needed.
    /// </summary>
    private static async Task<bool> AwaitLoadAsync(
        string format,
        Action<EventHandler, EventHandler<IAdError>> subscribe,
        Action<EventHandler, EventHandler<IAdError>> unsubscribe,
        Action load)
    {
        for (var attempt = 1; attempt <= Attempts; attempt++)
        {
            var tcs = new TaskCompletionSource<string?>(); // null message = loaded, otherwise the failure detail
            EventHandler onLoaded = (_, _) => tcs.TrySetResult(null);
            EventHandler<IAdError> onFailed = (_, e) => tcs.TrySetResult(e?.Message ?? "unknown error");

            subscribe(onLoaded, onFailed);

            try
            {
                await MainThread.InvokeOnMainThreadAsync(load);

                var finished = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds)));
                var detail = finished == tcs.Task ? await tcs.Task : $"timeout after {TimeoutSeconds}s";

                if (detail is null)
                {
                    HarnessLog.Line($"RESULT format={format} status=PASS attempt={attempt}");
                    return true;
                }

                HarnessLog.Line($"RESULT format={format} status={LastOrRetry(attempt)} attempt={attempt} detail=\"{Sanitize(detail)}\"");
            }
            catch (Exception ex)
            {
                HarnessLog.Line($"RESULT format={format} status={LastOrRetry(attempt)} attempt={attempt} detail=\"{Sanitize(ex.Message)}\"");
            }
            finally
            {
                unsubscribe(onLoaded, onFailed);
            }
        }

        return false;
    }

    /// <summary>
    /// Banner has no Load()/CreateAd() API — its handler auto-loads when the control enters a
    /// rendered visual tree, so each attempt uses a fresh control in a cleared host.
    /// </summary>
    private static async Task<bool> RunBannerAsync(Layout host)
    {
        for (var attempt = 1; attempt <= Attempts; attempt++)
        {
            var banner = new BannerAd { AdSize = AdSize.Banner };
            var tcs = new TaskCompletionSource<string?>();
            EventHandler onLoaded = (_, _) => tcs.TrySetResult(null);
            EventHandler<IAdError> onFailed = (_, e) => tcs.TrySetResult(e?.Message ?? "unknown error");

            banner.OnAdLoaded += onLoaded;
            banner.OnAdFailedToLoad += onFailed;

            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    host.Clear();
                    host.Add(banner);
                });

                var finished = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds)));
                var detail = finished == tcs.Task ? await tcs.Task : $"timeout after {TimeoutSeconds}s";

                if (detail is null)
                {
                    HarnessLog.Line($"RESULT format=banner status=PASS attempt={attempt}");
                    return true;
                }

                HarnessLog.Line($"RESULT format=banner status={LastOrRetry(attempt)} attempt={attempt} detail=\"{Sanitize(detail)}\"");
            }
            finally
            {
                banner.OnAdLoaded -= onLoaded;
                banner.OnAdFailedToLoad -= onFailed;
            }
        }

        return false;
    }

    private static string LastOrRetry(int attempt) => attempt < Attempts ? "RETRY" : "FAIL";

    private static string Sanitize(string s) => s.Replace('\r', ' ').Replace('\n', ' ').Replace('"', '\'');
}
