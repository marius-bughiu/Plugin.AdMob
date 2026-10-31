using Plugin.AdMob.Services;

namespace Plugin.AdMob.DeviceTests;

/// <summary>
/// Requests ads while the app is starting, before any consent request has run (the harness is a
/// fresh install). Both PrepareAd() and CreateAd().Load() must wait for consent and then load.
/// </summary>
internal static class EarlyLoads
{
    private static readonly TaskCompletionSource _prepared = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private static readonly TaskCompletionSource<DateTime> _loaded = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static string? _loadError;

    public static void Start(IServiceProvider services)
    {
        var interstitialAdService = services.GetRequiredService<IInterstitialAdService>();
        interstitialAdService.OnAdLoaded += (_, _) => _prepared.TrySetResult();
        interstitialAdService.PrepareAd();

        var ad = interstitialAdService.CreateAd();
        ad.OnAdLoaded += (_, _) => _loaded.TrySetResult(DateTime.UtcNow);
        ad.OnAdFailedToLoad += (_, error) => _loadError ??= error.Message;
        ad.Load();
    }

    public static async Task<bool> WaitForPreparedAsync(TimeSpan timeout)
        => await Task.WhenAny(_prepared.Task, Task.Delay(timeout)) == _prepared.Task;

    public static async Task<(bool Passed, string Detail)> WaitForLoadAsync(DateTime consentAt, TimeSpan timeout)
    {
        if (await Task.WhenAny(_loaded.Task, Task.Delay(timeout)) != _loaded.Task)
        {
            return (false, _loadError ?? $"no load within {timeout.TotalSeconds}s");
        }

        var loadedAt = await _loaded.Task;
        return loadedAt >= consentAt
            ? (true, "loaded after consent")
            : (false, "loaded before consent");
    }
}
