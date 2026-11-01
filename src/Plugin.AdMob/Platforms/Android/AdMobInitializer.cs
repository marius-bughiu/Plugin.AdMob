using Android.Content;
using Android.Content.PM;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.Initialization;
using System.Diagnostics;

namespace Plugin.AdMob.Platforms.Android;

internal static class AdMobInitializer
{
    private const string ApplicationIdMetadataKey = "com.google.android.gms.ads.APPLICATION_ID";

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
            StartInitialization();
        }
    }

    private static void StartInitialization()
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
                MobileAds.Initialize(context, config, new InitializationCompleteListener());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Plugin.AdMob] Mobile Ads SDK initialization failed: {ex.Message}");

                lock (_lock)
                {
                    _initializing = false;
                }
            }
        });
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

    private static void OnInitializationComplete()
    {
        List<Action> callbacks;

        lock (_lock)
        {
            _initialized = true;
            callbacks = [.. _pending];
            _pending.Clear();
        }

        MainThreadDispatcher.Run(() =>
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

    private sealed class InitializationCompleteListener : Java.Lang.Object, IOnAdapterInitializationCompleteListener
    {
        public void OnAdapterInitializationComplete(IInitializationStatus status) => OnInitializationComplete();
    }
}
