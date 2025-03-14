using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing interstitial ads.
/// </summary>
public interface IInterstitialAdService
{
    /// <summary>
    /// True when an ad is ready to be presented to the user.
    /// </summary>
    public bool IsAdLoaded { get; }

    /// <summary>
    /// Creates an interstitial ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultInterstitialAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>An interstitial ad instance.</returns>
    IInterstitialAd CreateAd(string? adUnitId = null);

    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultInterstitialAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    void PrepareAd(string? adUnitId = null);

    /// <summary>
    /// Displays the already prepared ad. Does nothing if no ad was prepared.
    /// </summary>
    void ShowAd();

    /// <summary>
    /// Raised when an ad is loaded, after calling <see cref="PrepareAd" />. 
    /// You can now call <see cref="ShowAd" /> to present the ad to the user.
    /// Note: This is not a catch-all event handler. When using <see cref="CreateAd" />, you should register to the ad loaded
    /// event handler of the IInterstitialAd returned by the method.
    /// </summary>
    event EventHandler OnAdLoaded;
}

internal class InterstitialAdService(IAdConsentService _adConsentService) 
    : IInterstitialAdService
{
    private IInterstitialAd? _interstitialAd;

    public bool IsAdLoaded { get; private set; }

    public event EventHandler? OnAdLoaded;

    public IInterstitialAd CreateAd(string? adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default interstitial ad unit ID has been configured.");
        }

        return new InterstitialAd(adUnitId);
    }

    public void PrepareAd(string? adUnitId = null)
    {
        if (_interstitialAd is not null)
        {
            IsAdLoaded = false;
            _interstitialAd.OnAdLoaded -= OnAdLoadedInternal;
        }

        if (CanRequestAds() is false)
        {
            return;
        }

        var interstitialAd = CreateAd(adUnitId);
        interstitialAd.OnAdLoaded += OnAdLoadedInternal;

        interstitialAd.Load();

        _interstitialAd = interstitialAd;
    }

    public void ShowAd()
    {
        IsAdLoaded = false;
        _interstitialAd?.Show();
    }

    private static string? GetAdUnitId(string? adUnitId)
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Interstitial;
        }

        return adUnitId ?? AdConfig.DefaultInterstitialAdUnitId;
    }

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService.CanRequestAds();
    }

    private void OnAdLoadedInternal(object? sender, EventArgs e)
    {
        IsAdLoaded = true;
        OnAdLoaded?.Invoke(sender, e);
    }
}
