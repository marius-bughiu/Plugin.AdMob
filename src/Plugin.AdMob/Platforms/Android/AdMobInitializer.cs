using Android.Content;
using Android.Content.PM;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.Initialization;
using System.Diagnostics;
using SdkIInitializationStatus = Google.Android.Libraries.Ads.Mobile.Sdk.Initialization.IInitializationStatus;

namespace Plugin.AdMob.Platforms.Android;

internal static class AdMobInitializer
{
    private const string ApplicationIdMetadataKey = "com.google.android.gms.ads.APPLICATION_ID";

    private static readonly object _lock = new();
    private static readonly List<AdRequestCallbacks> _pending = [];
    private static TaskCompletionSource<IInitializationStatus> _initialization = CreateInitialization();
    private static bool _initializing;
    private static bool _initialized;

    internal static Task<IInitializationStatus> Initialization
    {
        get
        {
            lock (_lock)
            {
                return _initialization.Task;
            }
        }
    }

    internal static void EnsureInitialized() => StartOrQueue(null);

    internal static void RunWhenInitialized(Action request, Action<Exception> onFailed)
        => StartOrQueue(new AdRequestCallbacks(request, onFailed));

    private static void StartOrQueue(AdRequestCallbacks? callbacks)
    {
        bool initialized;
        TaskCompletionSource<IInitializationStatus>? attempt = null;

        lock (_lock)
        {
            initialized = _initialized;

            if (!initialized)
            {
                if (callbacks is { } request)
                {
                    _pending.Add(request);
                }

                // After a failed attempt, a new ad request starts the retry.
                if (!_initializing && (callbacks.HasValue || !_initialization.Task.IsFaulted))
                {
                    if (_initialization.Task.IsFaulted)
                    {
                        _initialization = CreateInitialization();
                    }

                    _initializing = true;
                    attempt = _initialization;
                }
            }
        }

        if (initialized)
        {
            callbacks?.Run();
        }
        else if (attempt is not null)
        {
            StartInitialization(attempt);
        }
    }

    private static TaskCompletionSource<IInitializationStatus> CreateInitialization()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static void StartInitialization(TaskCompletionSource<IInitializationStatus> attempt)
    {
        try
        {
            var context = global::Android.App.Application.Context;
            var applicationId = ReadApplicationId(context);

            if (string.IsNullOrEmpty(applicationId))
            {
                throw new InvalidOperationException(
                    $"[Plugin.AdMob] The AdMob application ID is missing. Add a '<meta-data " +
                    $"android:name=\"{ApplicationIdMetadataKey}\" android:value=\"ca-app-pub-XXXXXXXX~YYYYYYYY\" />' " +
                    "entry to your Android app's AndroidManifest.xml.");
            }

            var requestConfiguration = new RequestConfiguration.Builder();
            requestConfiguration.ApplyGlobalAdConfiguration();

            var config = new InitializationConfig.Builder(applicationId)
                .SetRequestConfiguration(requestConfiguration.Build())
                .Build();

            Task.Run(() =>
            {
                try
                {
                    MobileAds.Initialize(context, config, new InitializationCompleteListener(attempt));
                }
                catch (Exception ex)
                {
                    OnInitializationFailed(attempt, ex);
                }
            });
        }
        catch (Exception ex)
        {
            OnInitializationFailed(attempt, ex);
        }
    }

    private static void OnInitializationFailed(TaskCompletionSource<IInitializationStatus> attempt, Exception exception)
    {
        List<AdRequestCallbacks> callbacks;
        lock (_lock)
        {
            if (!IsCurrentAttempt(attempt))
            {
                return;
            }

            _initializing = false;
            callbacks = DrainPendingRequests();
            attempt.TrySetException(exception);
        }

        Debug.WriteLine($"[Plugin.AdMob] Mobile Ads SDK initialization failed: {exception.Message}");
        foreach (var callback in callbacks)
        {
            callback.Fail(exception);
        }
    }

    private static string? ReadApplicationId(Context context)
    {
        try
        {
            var packageManager = context.PackageManager!;
            var packageName = context.PackageName!;

            ApplicationInfo applicationInfo;
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                applicationInfo = packageManager.GetApplicationInfo(
                    packageName,
                    PackageManager.ApplicationInfoFlags.Of((long)PackageInfoFlags.MetaData)!);
            }
            else
            {
#pragma warning disable CA1422 // Validate platform compatibility
                applicationInfo = packageManager.GetApplicationInfo(packageName, PackageInfoFlags.MetaData);
#pragma warning restore CA1422
            }

            return applicationInfo.MetaData?.GetString(ApplicationIdMetadataKey);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Plugin.AdMob] Failed to read the AdMob application ID from the manifest: {ex.Message}");
            return null;
        }
    }

    private static void OnInitializationComplete(TaskCompletionSource<IInitializationStatus> attempt, SdkIInitializationStatus status)
    {
        List<AdRequestCallbacks> callbacks;

        lock (_lock)
        {
            if (!IsCurrentAttempt(attempt))
            {
                return;
            }

            _initialized = true;
            _initializing = false;
            callbacks = DrainPendingRequests();
        }

        try
        {
            attempt.TrySetResult(status.ToInitializationStatus());
        }
        catch (Exception ex)
        {
            attempt.TrySetException(ex);
        }

        MainThreadDispatcher.Run(() =>
        {
            foreach (var callback in callbacks)
            {
                callback.Run();
            }
        });
    }

    private static bool IsCurrentAttempt(TaskCompletionSource<IInitializationStatus> attempt)
        => _initializing && ReferenceEquals(attempt, _initialization);

    private static List<AdRequestCallbacks> DrainPendingRequests()
    {
        var callbacks = new List<AdRequestCallbacks>(_pending);
        _pending.Clear();
        return callbacks;
    }

    private readonly record struct AdRequestCallbacks(Action Request, Action<Exception> OnFailed)
    {
        public void Run()
        {
            try
            {
                Request();
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        public void Fail(Exception exception)
        {
            var onFailed = OnFailed;
            MainThreadDispatcher.Post(() => onFailed(exception));
        }
    }

    private sealed class InitializationCompleteListener(TaskCompletionSource<IInitializationStatus> attempt)
        : Java.Lang.Object, IOnAdapterInitializationCompleteListener
    {
        public void OnAdapterInitializationComplete(SdkIInitializationStatus status) => OnInitializationComplete(attempt, status);
    }
}
