namespace Android.App
{
    internal static class Application
    {
        public static Content.Context Context { get; } = new();
    }
}

namespace Android.Content
{
    internal sealed class Context
    {
        public PM.PackageManager PackageManager { get; } = new();
        public string PackageName => "plugin.test";
    }
}

namespace Android.Content.PM
{
    internal enum PackageInfoFlags { MetaData = 128 }
    internal sealed class ApplicationInfo
    {
        public MetaData MetaData { get; } = new();
    }

    internal sealed class MetaData
    {
        public static string? AppId { get; set; } = "test-app-id";
        public string? GetString(string key) => AppId;
    }

    internal sealed class PackageManager
    {
        public sealed class ApplicationInfoFlags
        {
            public static ApplicationInfoFlags Of(long value) => new();
        }

        public ApplicationInfo GetApplicationInfo(string packageName, ApplicationInfoFlags flags) => new();
        public ApplicationInfo GetApplicationInfo(string packageName, PackageInfoFlags flags) => new();
    }
}

namespace Java.Lang { internal class Object { } }

namespace Google.Android.Libraries.Ads.Mobile.Sdk.Common
{
    internal sealed class RequestConfiguration
    {
        public sealed class Builder
        {
            public RequestConfiguration Build() => new();
        }
    }
}

namespace Google.Android.Libraries.Ads.Mobile.Sdk.Initialization
{
    internal interface IInitializationStatus { }
    internal sealed class Status : IInitializationStatus { }
    internal interface IOnAdapterInitializationCompleteListener
    {
        void OnAdapterInitializationComplete(IInitializationStatus status);
    }

    internal sealed class InitializationConfig
    {
        public sealed class Builder(string applicationId)
        {
            public Builder SetRequestConfiguration(Common.RequestConfiguration request) => this;
            public InitializationConfig Build() => applicationId.Length > 0 ? new() : throw new ArgumentException();
        }
    }
}

namespace Google.Android.Libraries.Ads.Mobile.Sdk
{
    internal static class MobileAds
    {
        public static System.Collections.Concurrent.ConcurrentQueue<Attempt> Attempts { get; } = new();
        public static void Initialize(global::Android.Content.Context context, Initialization.InitializationConfig config,
            Initialization.IOnAdapterInitializationCompleteListener listener)
        {
            var attempt = new Attempt(listener);
            Attempts.Enqueue(attempt);
            if (!attempt.AllowCompletion.Task.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult())
                throw new InvalidOperationException("Simulated SDK failure");

            attempt.CompleteLate();
        }

        internal sealed class Attempt(Initialization.IOnAdapterInitializationCompleteListener listener)
        {
            public TaskCompletionSource<bool> AllowCompletion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public void CompleteLate() => listener.OnAdapterInitializationComplete(new Initialization.Status());
        }
    }
}


namespace Plugin.AdMob.Platforms.Android
{
    internal static class Extensions
    {
        public static void ApplyGlobalAdConfiguration(
            this Google.Android.Libraries.Ads.Mobile.Sdk.Common.RequestConfiguration.Builder builder) { }

        public static Plugin.AdMob.IInitializationStatus ToInitializationStatus(
            this Google.Android.Libraries.Ads.Mobile.Sdk.Initialization.IInitializationStatus status) => new Plugin.AdMob.InitializationStatus([]);
    }

    internal static class MainThreadDispatcher
    {
        internal static void Run(Action action) => action();
        internal static void Post(Action action) => action();
    }
}
