using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.NativeAd;

namespace Plugin.AdMob.Platforms.Android.Native;

/// <summary>
/// Bridges the next-gen <see cref="INativeAdEventCallback" /> to plain C# events.
/// </summary>
internal sealed class NativeAdEventCallback : Java.Lang.Object, INativeAdEventCallback
{
    public event EventHandler? AdImpression;
    public event EventHandler? AdClicked;
    public event EventHandler? AdSwiped;
    public event EventHandler? AdOpened;
    public event EventHandler? AdClosed;

    public void OnAdImpression()
        => MainThreadDispatcher.Run(() => AdImpression?.Invoke(this, EventArgs.Empty));

    public void OnAdClicked()
        => MainThreadDispatcher.Run(() => AdClicked?.Invoke(this, EventArgs.Empty));

    public void OnAdSwipeGestureClicked()
        => MainThreadDispatcher.Run(() => AdSwiped?.Invoke(this, EventArgs.Empty));

    // A native ad "opens" by showing full screen content over the app, and "closes" when it is dismissed.
    public void OnAdShowedFullScreenContent()
        => MainThreadDispatcher.Run(() => AdOpened?.Invoke(this, EventArgs.Empty));

    public void OnAdDismissedFullScreenContent()
        => MainThreadDispatcher.Run(() => AdClosed?.Invoke(this, EventArgs.Empty));

    public void OnAdFailedToShowFullScreenContent(FullScreenContentError fullScreenContentError) { }

    // Not surfaced by the plugin.
    public void OnAdPaid(AdValue value) { }

    public void OnCustomMuteThisAdReported() { }
}
