namespace Plugin.AdMob.Platforms.Android.Native;

internal class VideoLifecycleCallbacks : global::Android.Gms.Ads.VideoController.VideoLifecycleCallbacks
{
    public event EventHandler? WhenVideoStarted;
    public event EventHandler? WhenVideoPlayed;
    public event EventHandler? WhenVideoPaused;
    public event EventHandler? WhenVideoEnded;
    public event EventHandler<bool>? WhenVideoMuted;

    public override void OnVideoStart()
    {
        base.OnVideoStart();
        WhenVideoStarted?.Invoke(this, EventArgs.Empty);
    }

    public override void OnVideoPlay()
    {
        base.OnVideoPlay();
        WhenVideoPlayed?.Invoke(this, EventArgs.Empty);
    }

    public override void OnVideoPause()
    {
        base.OnVideoPause();
        WhenVideoPaused?.Invoke(this, EventArgs.Empty);
    }

    public override void OnVideoEnd()
    {
        base.OnVideoEnd();
        WhenVideoEnded?.Invoke(this, EventArgs.Empty);
    }

    public override void OnVideoMute(bool isMuted)
    {
        base.OnVideoMute(isMuted);
        WhenVideoMuted?.Invoke(this, isMuted);
    }
}
