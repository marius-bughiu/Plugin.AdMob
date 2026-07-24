using Google.Android.Libraries.Ads.Mobile.Sdk.Banner;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;

namespace Plugin.AdMob.Platforms.Android.Banner;

/// <summary>
/// Bridges the next-gen <see cref="IBannerAdEventCallback" /> to plain C# events. The next-gen banner API only
/// reports click and impression events (plus paid/app events, which the plugin does not surface); the legacy
/// opened/closed/swiped events no longer exist.
/// </summary>
internal sealed class BannerAdEventCallback : Java.Lang.Object, IBannerAdEventCallback
{
    public event EventHandler? AdImpression;
    public event EventHandler? AdClicked;

    public void OnAdImpression()
        => MainThreadDispatcher.Run(() => AdImpression?.Invoke(this, EventArgs.Empty));

    public void OnAdClicked()
        => MainThreadDispatcher.Run(() => AdClicked?.Invoke(this, EventArgs.Empty));

    // Not applicable to banners / not surfaced by the plugin.
    public void OnAdShowedFullScreenContent() { }

    public void OnAdFailedToShowFullScreenContent(FullScreenContentError fullScreenContentError) { }

    public void OnAdDismissedFullScreenContent() { }

    public void OnAdPaid(AdValue value) { }

    public void OnAppEvent(string name, string? data) { }
}
