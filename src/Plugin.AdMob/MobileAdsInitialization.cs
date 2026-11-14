#if ANDROID || IOS
#if ANDROID
using Plugin.AdMob.Platforms.Android;
#else
using Plugin.AdMob.Platforms.iOS;
#endif

namespace Plugin.AdMob;

internal static class MobileAdsInitialization
{
    private static readonly TaskCompletionSource<IInitializationStatus> _completed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private static int _started;

    internal static Task<IInitializationStatus> Completed => _completed.Task;

    internal static void Start()
    {
        if (Interlocked.Exchange(ref _started, 1) == 1)
        {
            return;
        }

        AdMobInitializer.EnsureInitialized();
    }

    internal static void Complete(Func<IInitializationStatus> getStatus)
    {
        try
        {
            _completed.TrySetResult(getStatus());
        }
        catch (Exception ex)
        {
            _completed.TrySetException(ex);
        }
    }
}
#endif
