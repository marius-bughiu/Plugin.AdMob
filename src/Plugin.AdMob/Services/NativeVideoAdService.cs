using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing native video ads.
/// </summary>
public interface INativeVideoAdService
{
    /// <summary>
    /// Creates a native video ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultNativeVideoAdUnitId" /> will be used.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <returns>A native video ad instance.</returns>
    INativeAd CreateAd(string? adUnitId = null);
}

internal class NativeVideoAdService : INativeVideoAdService
{
    public INativeAd CreateAd(string? adUnitId = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default native video ad unit ID has been configured.");
        }

        return new NativeAd(adUnitId);
    }

    private static string? GetAdUnitId(string? adUnitId)
    {
#if ANDROID || IOS
        if (AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.NativeVideo;
        }
#endif

        return adUnitId ?? AdConfig.DefaultNativeVideoAdUnitId;
    }
}
