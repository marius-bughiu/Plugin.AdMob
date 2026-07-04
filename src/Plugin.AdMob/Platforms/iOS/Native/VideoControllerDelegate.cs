using Google.MobileAds;

namespace Plugin.AdMob.Platforms.iOS;

internal class NativeVideoControllerDelegate : VideoControllerDelegate
{
    public event EventHandler? WhenVideoPlayed;
    public event EventHandler? WhenVideoPaused;
    public event EventHandler? WhenVideoEnded;
    public event EventHandler<bool>? WhenVideoMuted;

    public override void DidPlayVideo(VideoController videoController)
        => WhenVideoPlayed?.Invoke(this, EventArgs.Empty);

    public override void DidPauseVideo(VideoController videoController)
        => WhenVideoPaused?.Invoke(this, EventArgs.Empty);

    public override void DidEndVideoPlayback(VideoController videoController)
        => WhenVideoEnded?.Invoke(this, EventArgs.Empty);

    public override void DidMuteVideo(VideoController videoController)
        => WhenVideoMuted?.Invoke(this, true);

    public override void DidUnmuteVideo(VideoController videoController)
        => WhenVideoMuted?.Invoke(this, false);
}
