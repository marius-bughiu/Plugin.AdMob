namespace Plugin.AdMob;

public interface IInterstitialAd
{
    string AdUnitId { get; }

    bool IsLoaded { get; }

    event EventHandler OnAdLoaded;
    event EventHandler<IAdError> OnAdFailedToLoad;
    event EventHandler OnAdShowed;
    event EventHandler<IAdError> OnAdFailedToShow;
    event EventHandler OnAdImpression;
    event EventHandler OnAdClicked;
    event EventHandler OnAdDismissed;

    void Load() => throw new NotImplementedException();

    void Show() => throw new NotImplementedException();
}

internal partial class InterstitialAd : IInterstitialAd
{
    public string AdUnitId { get; }

    public bool IsLoaded { get; private set; }

    public event EventHandler OnAdLoaded;
    public event EventHandler<IAdError> OnAdFailedToLoad;
    public event EventHandler OnAdShowed;
    public event EventHandler<IAdError> OnAdFailedToShow;
    public event EventHandler OnAdImpression;
    public event EventHandler OnAdClicked;
    public event EventHandler OnAdDismissed;

    public InterstitialAd(string adUnitId)
    {
        AdUnitId = adUnitId;
    }
}
