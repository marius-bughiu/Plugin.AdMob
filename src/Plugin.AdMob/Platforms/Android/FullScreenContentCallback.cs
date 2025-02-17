namespace Plugin.AdMob;

internal class FullScreenContentCallback : global::Android.Gms.Ads.FullScreenContentCallback
{
    public event EventHandler? AdPresented;
    public event EventHandler<global::Android.Gms.Ads.AdError>? AdFailedToPresent;
    public event EventHandler? AdImpression;
    public event EventHandler? AdClicked;
    public event EventHandler? AdClosed;

    public override void OnAdShowedFullScreenContent()
    {
        base.OnAdShowedFullScreenContent();
        AdPresented?.Invoke(this, EventArgs.Empty);
    }

    public override void OnAdFailedToShowFullScreenContent(global::Android.Gms.Ads.AdError error)
    {
        base.OnAdFailedToShowFullScreenContent(error);
        AdFailedToPresent?.Invoke(this, error);
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
        AdClosed?.Invoke(this, EventArgs.Empty);
    }
}