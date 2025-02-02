namespace Plugin.AdMob;

/// <summary>
/// Values for ad size.
/// </summary>
public enum AdSize
{
    /// <summary>
    /// A dynamically sized banner that is full-width and auto-height.
    /// </summary>
    SmartBanner,

    /// <summary>
    /// Mobile Marketing Association (MMA) banner ad size (320x50 density-independent pixels).
    /// </summary>
    Banner,

    /// <summary>
    /// Large banner ad size (320x100 density-independent pixels).
    /// </summary>
    LargeBanner,

    /// <summary>
    /// Interactive Advertising Bureau (IAB) medium rectangle ad size (300x250 density-independent pixels).
    /// </summary>
    MediumRectangle,

    /// <summary>
    /// Interactive Advertising Bureau (IAB) full banner ad size (468x60 density-independent pixels).
    /// </summary>
    FullBanner,

    /// <summary>
    /// Interactive Advertising Bureau (IAB) leaderboard ad size (728x90 density-independent pixels).
    /// </summary>
    Leaderboard,

    /// <summary>
    /// A custom sized banner for which you can specify the desired width and height.
    /// </summary>
    Custom
}
