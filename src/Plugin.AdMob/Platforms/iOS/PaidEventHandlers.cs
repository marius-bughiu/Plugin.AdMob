using Google.MobileAds;

namespace Plugin.AdMob.Platforms.iOS;

internal static class PaidEventHandlers
{
    internal static PaidEventHandler Create<TAd>(TAd ad, Action<TAd, AdValue> onPaid)
        where TAd : class
    {
        // The native ad owns this block. Callers use a static callback so the block
        // cannot keep the managed wrapper (and therefore the native ad) alive.
        var weakAd = new WeakReference<TAd>(ad);
        return value =>
        {
            if (weakAd.TryGetTarget(out var liveAd))
            {
                onPaid(liveAd, value);
            }
        };
    }
}
