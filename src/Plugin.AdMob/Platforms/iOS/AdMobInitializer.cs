using CoreFoundation;
using Google.MobileAds;
using System.Diagnostics;

namespace Plugin.AdMob.Platforms.iOS;

internal static class AdMobInitializer
{
    private static readonly object _lock = new();
    private static readonly List<Action> _pending = [];
    private static bool _initializing;
    private static bool _initialized;

    internal static void EnsureInitialized() => RunWhenInitialized(null);

    internal static void RunWhenInitialized(Action? onInitialized)
    {
        bool initialized;
        var start = false;

        lock (_lock)
        {
            initialized = _initialized;

            if (!initialized)
            {
                if (onInitialized is not null)
                {
                    _pending.Add(onInitialized);
                }

                start = !_initializing;
                _initializing = true;
            }
        }

        if (initialized)
        {
            onInitialized?.Invoke();
        }
        else if (start)
        {
            MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
            MobileAds.SharedInstance.Start(_ => OnInitializationComplete());
        }
    }

    private static void OnInitializationComplete()
    {
        List<Action> callbacks;

        lock (_lock)
        {
            _initialized = true;
            callbacks = [.. _pending];
            _pending.Clear();
        }

        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            foreach (var callback in callbacks)
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Plugin.AdMob] A queued ad request failed after initialization: {ex.Message}");
                }
            }
        });
    }
}
