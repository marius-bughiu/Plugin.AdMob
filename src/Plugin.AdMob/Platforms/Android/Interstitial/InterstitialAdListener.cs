using Android.Gms.Ads;

namespace Plugin.AdMob.Platforms.Android.Interstitial
{
    internal class InterstitialAdListener : FullScreenContentCallback
    {
        public event EventHandler AdShowed;
        public event EventHandler<global::Android.Gms.Ads.AdError> AdFailedToShow;
        public event EventHandler AdImpression;
        public event EventHandler AdClicked;
        public event EventHandler AdDismissed;

        public override void OnAdShowedFullScreenContent()
        {
            base.OnAdShowedFullScreenContent();
            AdShowed?.Invoke(this, EventArgs.Empty);
        }

        public override void OnAdFailedToShowFullScreenContent(global::Android.Gms.Ads.AdError error)
        {
            base.OnAdFailedToShowFullScreenContent(error);
            AdFailedToShow?.Invoke(this, error);
        }

        public override void OnAdImpression()
        {
            base.OnAdImpression();
            AdImpression?.Invoke(this, EventArgs.Empty);
        }

        public override void OnAdClicked()
        {
            base.OnAdClicked();
            AdClicked?.Invoke(this, EventArgs.Empty);
        }

        public override void OnAdDismissedFullScreenContent()
        {
            base.OnAdDismissedFullScreenContent();
            AdDismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
