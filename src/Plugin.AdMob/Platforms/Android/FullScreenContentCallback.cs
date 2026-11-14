using Google.Android.Libraries.Ads.Mobile.Sdk.AppOpen;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.Interstitial;
using Google.Android.Libraries.Ads.Mobile.Sdk.Rewarded;
using Google.Android.Libraries.Ads.Mobile.Sdk.RewardedInterstitial;
using Plugin.AdMob.Platforms.Android;

namespace Plugin.AdMob;

/// <summary>
/// Bridges the next-gen full-screen ad event callbacks to plain C# events. A single instance implements the
/// event-callback interface of every full-screen format, so it can be assigned to any ad's
/// <c>AdEventCallback</c> property.
/// </summary>
internal sealed class FullScreenContentCallback :
    Java.Lang.Object,
    IInterstitialAdEventCallback,
    IAppOpenAdEventCallback,
    IRewardedAdEventCallback,
    IRewardedInterstitialAdEventCallback
{
    public event EventHandler? AdShowed;
    public event EventHandler<FullScreenContentError>? AdFailedToShow;
    public event EventHandler? AdImpression;
    public event EventHandler? AdClicked;
    public event EventHandler? AdDismissed;

    public void OnAdShowedFullScreenContent()
        => MainThreadDispatcher.Run(() => AdShowed?.Invoke(this, EventArgs.Empty));

    public void OnAdFailedToShowFullScreenContent(FullScreenContentError fullScreenContentError)
        => MainThreadDispatcher.Run(() => AdFailedToShow?.Invoke(this, fullScreenContentError));

    public void OnAdImpression()
        => MainThreadDispatcher.Run(() => AdImpression?.Invoke(this, EventArgs.Empty));

    public void OnAdClicked()
        => MainThreadDispatcher.Run(() => AdClicked?.Invoke(this, EventArgs.Empty));

    public void OnAdDismissedFullScreenContent()
        => MainThreadDispatcher.Run(() => AdDismissed?.Invoke(this, EventArgs.Empty));

    // Revenue callback is not surfaced by the plugin's public API.
    public void OnAdPaid(AdValue value) { }

    // IOnAppEventListener (interstitial) — not surfaced by the plugin.
    public void OnAppEvent(string name, string? data) { }

    // IOnAdMetadataChangedListener (rewarded / rewarded interstitial) — not surfaced by the plugin.
    public void OnAdMetadataChanged() { }
}
