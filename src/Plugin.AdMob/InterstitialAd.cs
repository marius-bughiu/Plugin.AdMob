namespace Plugin.AdMob;

public interface IInterstitialAd
{
    string AdUnitId { get; }

    bool IsLoaded { get; }

    /// <summary>
    /// Raised when an ad is loaded.
    /// </summary>
    event EventHandler OnAdLoaded;

    /// <summary>
    /// Raised when an ad request failed.
    /// </summary>
    event EventHandler<IAdError> OnAdFailedToLoad;

    /// <summary>
    /// Raised when the ad showed the full screen content.
    /// </summary>
    event EventHandler OnAdShowed;

    /// <summary>
    /// Raised when the ad failed to show full screen content.
    /// </summary>
    event EventHandler<IAdError> OnAdFailedToShow;

    /// <summary>
    /// Raised when an impression is recorded for an ad.
    /// </summary>
    event EventHandler OnAdImpression;

    /// <summary>
    /// Raised when a click is recorded for an ad.
    /// </summary>
    event EventHandler OnAdClicked;

    /// <summary>
    /// Raised when the ad dismissed full screen content.
    /// </summary>
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
