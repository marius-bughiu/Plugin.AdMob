using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing app open ads.
/// </summary>
public interface IAppOpenAdService
{
    /// <summary>
    /// Creates an app open ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultAppOpenAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>An app open ad instance.</returns>
    IAppOpenAd CreateAd(string? adUnitId = null);

    /// <summary>
    /// Preloads an ad given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultAppOpenAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    void PrepareAd(string? adUnitId = null);

    /// <summary>
    /// Displays the already prepared ad. Does nothing if no ad was prepared.
    /// </summary>
    void ShowAd();
}

internal class AppOpenAdService(IAdConsentService _adConsentService) 
    : IAppOpenAdService
{
    private IAppOpenAd? _appOpenAd;

    public IAppOpenAd CreateAd(string? adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default app open ad unit ID has been configured.");
        }

        return new AppOpenAd(adUnitId);
    }

    public void PrepareAd(string? adUnitId = null)
    {
        if (CanRequestAds() is false)
        {
            return;
        }

        var appOpenAd = CreateAd(adUnitId);
        appOpenAd.Load();

        _appOpenAd = appOpenAd;
    }

    public void ShowAd()
    {
        _appOpenAd?.Show();
    }

    private static string? GetAdUnitId(string? adUnitId)
    {
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.OpenApp;
        }

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
}
