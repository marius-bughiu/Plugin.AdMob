using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing app open ads.
/// </summary>
public interface IAppOpenAdService
{
    /// <summary>
    /// True when an ad is ready to be presented to the user. Turns false once the ad is shown or expires.
    /// </summary>
    public bool IsAdLoaded { get; }

    /// <summary>
    /// Creates an app open ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultAppOpenAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>An app open ad instance.</returns>
    IAppOpenAd CreateAd(string? adUnitId = null);

    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultAppOpenAdUnitId" /> will be used. If consent doesn't allow ad requests yet, the ad is loaded as soon as it does.
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
    /// event handler of the IAppOpenAd returned by the method.
    /// </summary>
    event EventHandler OnAdLoaded;
}

internal class AppOpenAdService(IAdConsentService _adConsentService) 
    : IAppOpenAdService
{
    private IAppOpenAd? _appOpenAd;
    private string? _pendingAdUnitId;
    private bool _isShowingAd;

    public bool IsAdLoaded => _appOpenAd?.IsLoaded ?? false;

    public event EventHandler? OnAdLoaded;

    public IAppOpenAd CreateAd(string? adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default app open ad unit ID has been configured.");
        }

        return new AppOpenAd(adUnitId, CanRequestAds);
    }

    public void PrepareAd(string? adUnitId = null)
    {
        _adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;

        if (_appOpenAd is not null)
        {
            _appOpenAd.OnAdLoaded -= OnAdLoadedInternal;
            _appOpenAd = null;
        }

        if (CanRequestAds() is false)
        {
            _pendingAdUnitId = adUnitId;
            _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
            return;
        }

        var appOpenAd = CreateAd(adUnitId);
        appOpenAd.OnAdLoaded += OnAdLoadedInternal;

        appOpenAd.Load();

        _appOpenAd = appOpenAd;
    }

    public void ShowAd()
    {
        if (_isShowingAd || _appOpenAd is not { IsLoaded: true } appOpenAd)
        {
            return;
        }

        _isShowingAd = true;
        appOpenAd.OnAdDismissed += (_, _) => _isShowingAd = false;
        appOpenAd.OnAdFailedToShow += (_, _) => _isShowingAd = false;
        appOpenAd.Show();
    }

    private static string? GetAdUnitId(string? adUnitId)
    {
#if ANDROID || IOS
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.OpenApp;
        }
#endif

        return adUnitId ?? AdConfig.DefaultAppOpenAdUnitId;
    }

    private bool CanRequestAds()
    {
        if (AdConfig.DisableConsentCheck)
        {
            return true;
        }

        return _adConsentService.CanRequestAds();
    }

    private void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
    {
        if (CanRequestAds())
        {
            PrepareAd(_pendingAdUnitId);
        }
    }

    private void OnAdLoadedInternal(object? sender, EventArgs e)
    {
        OnAdLoaded?.Invoke(sender, e);
    }
}
