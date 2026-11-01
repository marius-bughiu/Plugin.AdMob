using Google.Android.Libraries.Ads.Mobile.Sdk.Banner;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;

namespace Plugin.AdMob.Platforms.Android.Banner;

/// <summary>
/// Bridges the next-gen <see cref="IBannerAdEventCallback" /> to plain C# events. A banner "opens" when a click
/// shows full screen content over the app, and "closes" when that content is dismissed. The legacy swipe-to-click
/// event has no next-gen equivalent for banners (<see cref="IBannerAdEventCallback" /> has no
/// <c>OnAdSwipeGestureClicked</c>, unlike the native ad callback).
/// </summary>
internal sealed class BannerAdEventCallback : Java.Lang.Object, IBannerAdEventCallback
{
    public event EventHandler? AdImpression;
    public event EventHandler? AdClicked;
    public event EventHandler? AdOpened;
    public event EventHandler? AdClosed;

    public void OnAdImpression()
        => MainThreadDispatcher.Run(() => AdImpression?.Invoke(this, EventArgs.Empty));

    public void OnAdClicked()
        => MainThreadDispatcher.Run(() => AdClicked?.Invoke(this, EventArgs.Empty));

    public void OnAdShowedFullScreenContent()
        => MainThreadDispatcher.Run(() => AdOpened?.Invoke(this, EventArgs.Empty));

    public void OnAdDismissedFullScreenContent()
        => MainThreadDispatcher.Run(() => AdClosed?.Invoke(this, EventArgs.Empty));

    // Not surfaced by the plugin.
    public void OnAdFailedToShowFullScreenContent(FullScreenContentError fullScreenContentError) { }

    public void OnAdPaid(AdValue value) { }

    public void OnAppEvent(string name, string? data) { }
}
