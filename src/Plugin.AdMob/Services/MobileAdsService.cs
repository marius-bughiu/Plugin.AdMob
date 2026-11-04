using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// Reports when the plugin has started the Google Mobile Ads SDK.
/// </summary>
public interface IMobileAdsService
{
    /// <summary>
    /// Completes once the plugin has started the Google Mobile Ads SDK and its mediation adapters have initialized or timed out,
    /// with the status of each adapter. The plugin starts the SDK once consent allows ad requests, or right before the first ad
    /// request when <see cref="AdConfig.DisableConsentCheck" /> is true, so this doesn't complete before then. It never completes
    /// on platforms other than Android and iOS. If the SDK's status can't be read, the task fails with that exception.
    /// </summary>
    Task<IInitializationStatus> Initialization { get; }
}

internal class MobileAdsService : IMobileAdsService
{
#if ANDROID || IOS
    public Task<IInitializationStatus> Initialization => MobileAdsInitialization.Completed;
#else
    public Task<IInitializationStatus> Initialization { get; } = new TaskCompletionSource<IInitializationStatus>().Task;
#endif
}
