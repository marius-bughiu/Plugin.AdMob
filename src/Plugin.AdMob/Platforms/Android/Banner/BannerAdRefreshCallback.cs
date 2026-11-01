using Google.Android.Libraries.Ads.Mobile.Sdk.Banner;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;

namespace Plugin.AdMob.Platforms.Android.Banner;

internal sealed class BannerAdRefreshCallback : Java.Lang.Object, IBannerAdRefreshCallback
{
    public event EventHandler? AdRefreshed;
    public event EventHandler<LoadAdError>? AdFailedToRefresh;

    public void OnAdRefreshed()
        => MainThreadDispatcher.Run(() => AdRefreshed?.Invoke(this, EventArgs.Empty));

    public void OnAdFailedToRefresh(LoadAdError adError)
        => MainThreadDispatcher.Run(() => AdFailedToRefresh?.Invoke(this, adError));
}
