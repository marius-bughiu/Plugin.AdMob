using Android.Content;
using Android.Content.PM;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Initialization;
using System.Diagnostics;

namespace Plugin.AdMob.Platforms.Android;

/// <summary>
/// The Google Mobile Ads Next-Gen SDK no longer initializes itself automatically from the
/// manifest. It must be initialized explicitly via <see cref="MobileAds.Initialize(Context, InitializationConfig)" />,
/// supplying the AdMob application ID programmatically. This helper reads the application ID from the
/// host app's existing <c>com.google.android.gms.ads.APPLICATION_ID</c> manifest metadata (so consumers
/// don't need to change anything), kicks off initialization once, and defers ad requests until it completes.
/// </summary>
internal static class AdMobInitializer
{
    private const string ApplicationIdMetadataKey = "com.google.android.gms.ads.APPLICATION_ID";

    private static readonly object _lock = new();
    private static readonly List<Action> _pending = [];
    private static bool _initializing;
    private static bool _initialized;

    /// <summary>
    /// Whether the Mobile Ads SDK has finished initializing.
    /// </summary>
    internal static bool IsInitialized
    {
        get { lock (_lock) { return _initialized; } }
    }

    /// <summary>
    /// Kicks off SDK initialization if it hasn't started yet. Safe to call multiple times.
    /// </summary>
    internal static void EnsureInitialized() => RunWhenInitialized(null);

    /// <summary>
    /// Runs <paramref name="onInitialized" /> once the SDK is initialized, kicking off initialization
    /// if required. If the SDK is already initialized the callback runs synchronously.
    /// </summary>
    internal static void RunWhenInitialized(Action? onInitialized)
    {
        lock (_lock)
        {
            if (!_initialized)
            {
                if (onInitialized is not null)
                {
                    _pending.Add(onInitialized);
                }

                StartInitialization();
                return;
            }
        }

        onInitialized?.Invoke();
    }

    private static void StartInitialization()
    {
        // Caller holds _lock.
        if (_initializing)
        {
            return;
        }

        _initializing = true;

        var context = global::Android.App.Application.Context;
        var applicationId = ReadApplicationId(context);

        if (string.IsNullOrEmpty(applicationId))
        {
            throw new InvalidOperationException(
                $"[Plugin.AdMob] The AdMob application ID is missing. Add a '<meta-data " +
                $"android:name=\"{ApplicationIdMetadataKey}\" android:value=\"ca-app-pub-XXXXXXXX~YYYYYYYY\" />' " +
                "entry to your Android app's AndroidManifest.xml.");
        }

        var config = new InitializationConfig.Builder(applicationId).Build();

        // Initialize is safe to call from the main thread; the SDK performs its work asynchronously and
        // reports completion via the listener. Ad requests issued before completion are queued by the SDK.
        MobileAds.Initialize(context, config, new InitializationCompleteListener());
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

        // Initialization completes on a background thread, but the queued work loads ads and updates
        // MAUI views, so it has to run on the main thread.
        MainThreadDispatcher.Run(() =>
        {
            foreach (var callback in callbacks)
            {
                callback();
            }
        });
    }

    private sealed class InitializationCompleteListener : Java.Lang.Object, IOnAdapterInitializationCompleteListener
    {
        public void OnAdapterInitializationComplete(IInitializationStatus status) => OnInitializationComplete();
    }
}
