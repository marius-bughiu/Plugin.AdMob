namespace Plugin.AdMob.Configuration;

/// <summary>
/// Options for controlling video playback in supported ad formats.
/// </summary>
public class VideoOptions
{
    /// <summary>
    /// Whether videos should start muted. Defaults to true.
    /// </summary>
    public bool StartMuted { get; set; } = true;

    /// <summary>
    /// Whether the requested video should have custom controls enabled for play/pause/mute/unmute.
    /// </summary>
    public bool CustomControlsRequested { get; set; }

    /// <summary>
    /// Whether the requested video should have the click to expand behavior.
    /// </summary>
    public bool ClickToExpandRequested { get; set; }
}
