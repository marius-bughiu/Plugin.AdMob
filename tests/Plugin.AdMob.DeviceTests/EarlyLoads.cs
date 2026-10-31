using Plugin.AdMob.Services;

namespace Plugin.AdMob.DeviceTests;

/// <summary>
/// Requests ads while the app is starting, before any consent request has run (the harness is a
/// fresh install). PrepareAd() must load once consent allows it; CreateAd().Load() must fail with
/// a consent error instead of requesting an ad.
/// </summary>
internal static class EarlyLoads
{
    private static readonly TaskCompletionSource _prepared = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static string? _loadResult;

    public static void Start(IServiceProvider services)
    {
        var interstitialAdService = services.GetRequiredService<IInterstitialAdService>();
        interstitialAdService.OnAdLoaded += (_, _) => _prepared.TrySetResult();
        interstitialAdService.PrepareAd();

        var ad = interstitialAdService.CreateAd();
        ad.OnAdLoaded += (_, _) => _loadResult ??= "loaded before consent";
        ad.OnAdFailedToLoad += (_, error) => _loadResult ??= error.Message;
        ad.Load();
    }

    public static (bool Passed, string Detail) LoadBeforeConsent()
        => (_loadResult?.Contains("consent", StringComparison.OrdinalIgnoreCase) is true, _loadResult ?? "no result");

    public static async Task<bool> WaitForPreparedAsync(TimeSpan timeout)
        => await Task.WhenAny(_prepared.Task, Task.Delay(timeout)) == _prepared.Task;
}
