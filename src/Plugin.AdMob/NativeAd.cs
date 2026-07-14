using Plugin.AdMob.Configuration;

namespace Plugin.AdMob;

/// <summary>
/// Manages a native ad instance.
/// </summary>
public interface INativeAd
{
    /// <summary>
    /// The ad unit ID.
    /// </summary>
    string AdUnitId { get; }

    /// <summary>
    /// Determines whether the ad is loaded or not.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Returns text that identifies the advertiser. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? Advertiser => throw new NotImplementedException();

    /// <summary>
    /// Returns body text. Apps are required to display this asset.
    /// </summary>
    string? Body => throw new NotImplementedException();

    /// <summary>
    /// Returns the ad's call to action (such as "Buy" or "Install"). Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? CallToAction => throw new NotImplementedException();

    /// <summary>
    /// Returns the primary text headline. Apps are required to display this asset.
    /// </summary>
    string? Headline => throw new NotImplementedException();

    //string? Icon => throw new NotImplementedException();

    /// <summary>
    /// Returns a the uri of a small image identifying the advertiser. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? IconUri => throw new NotImplementedException();

    //string? Images => throw new NotImplementedException();

    /// <summary>
    /// Returns a the uri of a large image identifying the advertiser. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? ImageUri => throw new NotImplementedException();

    /// <summary>
    /// For ads about apps, returns a string representing how much the app costs. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? Price => throw new NotImplementedException();

    /// <summary>
    /// For ads about apps, returns a star rating from 0 to 5 representing how many stars the app has in the store offering it. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    double? StarRating => throw new NotImplementedException();

    /// <summary>
    /// For ads about apps, returns the name of the store offering the app for download. For example, "Google Play". Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? Store => throw new NotImplementedException();

    /// <summary>
    /// The <see cref="Configuration.VideoOptions" /> this ad was requested with, or <c>null</c> when none were specified. The ad's media content
    /// itself is rendered by placing a <see cref="MediaView" /> inside the ad content template.
    /// </summary>
    VideoOptions? VideoOptions => null;

    /// <summary>
    /// Returns true when the loaded ad's media content is a video. Media content is displayed by placing a <see cref="MediaView" /> inside the ad content template.
    /// </summary>
    bool HasVideoContent => throw new NotImplementedException();

    /// <summary>
    /// Returns the aspect ratio of the loaded ad's media content (width/height), or 0 if unknown.
    /// </summary>
    double VideoAspectRatio => throw new NotImplementedException();

    /// <summary>
    /// Returns the duration of the ad's video, or <see cref="TimeSpan.Zero" /> when there is no video content.
    /// </summary>
    TimeSpan VideoDuration => throw new NotImplementedException();

    /// <summary>
    /// Returns the current playback time of the ad's video, or <see cref="TimeSpan.Zero" /> when there is no video content.
    /// </summary>
    TimeSpan VideoCurrentTime => throw new NotImplementedException();

    /// <summary>
    /// Returns true when the ad's video is currently muted, or false when there is no video content.
    /// </summary>
    bool IsVideoMuted => throw new NotImplementedException();

    /// <summary>
    /// Returns true when custom video controls (play/pause/mute) are enabled for the ad's video. Custom controls must be
    /// requested via <see cref="Configuration.VideoOptions.CustomControlsRequested" /> and supported by the loaded ad.
    /// </summary>
    bool VideoCustomControlsEnabled => throw new NotImplementedException();

    /// <summary>
    /// Returns true when click-to-expand behavior is enabled for the ad's video.
    /// </summary>
    bool VideoClickToExpandEnabled => throw new NotImplementedException();

    /// <summary>
    /// Plays the ad's video. Does nothing unless <see cref="VideoCustomControlsEnabled" /> is true and the video is paused.
    /// </summary>
    void PlayVideo() => throw new NotImplementedException();

    /// <summary>
    /// Pauses the ad's video. Does nothing unless <see cref="VideoCustomControlsEnabled" /> is true and the video is playing.
    /// </summary>
    void PauseVideo() => throw new NotImplementedException();

    /// <summary>
    /// Mutes or unmutes the ad's video. Does nothing unless <see cref="VideoCustomControlsEnabled" /> is true.
    /// </summary>
    void SetVideoMuted(bool muted) => throw new NotImplementedException();

    /// <summary>
    /// Raised when an ad is loaded.
    /// </summary>
    event EventHandler OnAdLoaded;

    /// <summary>
    /// Raised when an ad request failed.
    /// </summary>
    event EventHandler<IAdError> OnAdFailedToLoad;

    /// <summary>
    /// Raised when an impression is recorded for an ad.
    /// </summary>
    event EventHandler? OnAdImpression;

    /// <summary>
    /// Raised when a click is recorded for an ad.
    /// </summary>
    event EventHandler? OnAdClicked;

    /// <summary>
    /// Raised when a swipe gesture on an ad is recorded as a click. Supported only by Android.
    /// </summary>
    event EventHandler? OnAdSwiped;

    /// <summary>
    /// Raised when an ad opens an overlay that covers the screen.
    /// </summary>
    event EventHandler? OnAdOpened;

    /// <summary>
    /// Raised when the user is about to return to the application after clicking on an ad.
    /// </summary>
    event EventHandler? OnAdClosed;

    /// <summary>
    /// Raised when video playback first begins. Supported only by Android.
    /// </summary>
    event EventHandler? OnVideoStart;

    /// <summary>
    /// Raised when video playback is playing.
    /// </summary>
    event EventHandler? OnVideoPlay;

    /// <summary>
    /// Raised when video playback is paused.
    /// </summary>
    event EventHandler? OnVideoPause;

    /// <summary>
    /// Raised when video playback finishes playing.
    /// </summary>
    event EventHandler? OnVideoEnd;

    /// <summary>
    /// Raised when the video changes mute state. The argument is true when the video was muted.
    /// </summary>
    event EventHandler<bool>? OnVideoMuted;

    /// <summary>
    /// Loads a native ad using the specified <see cref="AdUnitId" />.
    /// </summary>
    void Load() => throw new NotImplementedException();
}

internal partial class NativeAd(string adUnitId, VideoOptions? videoOptions = null) : INativeAd
{
    public string AdUnitId { get; } = adUnitId;

    public VideoOptions? VideoOptions { get; } = videoOptions;

    public bool IsLoaded { get; private set; }

    public event EventHandler? OnAdLoaded;
    public event EventHandler<IAdError>? OnAdFailedToLoad;
    public event EventHandler? OnAdImpression;
    public event EventHandler? OnAdClicked;
    public event EventHandler? OnAdSwiped;
    public event EventHandler? OnAdOpened;
    public event EventHandler? OnAdClosed;
    public event EventHandler? OnVideoStart;
    public event EventHandler? OnVideoPlay;
    public event EventHandler? OnVideoPause;
    public event EventHandler? OnVideoEnd;
    public event EventHandler<bool>? OnVideoMuted;
}
