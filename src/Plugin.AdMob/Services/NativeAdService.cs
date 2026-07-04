using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

/// <summary>
/// A service used for managing native ads.
/// </summary>
public interface INativeAdService
{
    /// <summary>
    /// Creates a native ad instance given the specified ad unit ID. If no ad unit ID is specified, <see cref="AdConfig.DefaultNativeAdUnitId" /> will be used
    /// (or Google's native test ad unit when <see cref="AdConfig.UseTestAdUnitIds" /> is enabled). An explicitly specified ad unit ID always wins over
    /// <see cref="AdConfig.UseTestAdUnitIds" />, so ad units such as Google's native video test unit can be requested during development.
    /// </summary>
    /// <param name="adUnitId">The ad unit ID.</param>
    /// <param name="videoOptions">Optional video playback options, used when the ad unit serves video media content.</param>
    /// <returns>A native ad instance.</returns>
    INativeAd CreateAd(string? adUnitId = null, VideoOptions? videoOptions = null);
}

internal class NativeAdService : INativeAdService
{
    public INativeAd CreateAd(string? adUnitId = null, VideoOptions? videoOptions = null)
    {
        adUnitId = GetAdUnitId(adUnitId);

        if (adUnitId is null)
        {
            throw new ArgumentNullException(nameof(adUnitId), "No ad unit ID was specified, and no default native ad unit ID has been configured.");
        }

        return new NativeAd(adUnitId, videoOptions);
    }

    private static string? GetAdUnitId(string? adUnitId)
    {
#if ANDROID || IOS
        if (adUnitId is null && AdConfig.UseTestAdUnitIds)
        {
            return AdMobTestAdUnits.Native;
        }
#endif

        return adUnitId ?? AdConfig.DefaultNativeAdUnitId;
    }
}
