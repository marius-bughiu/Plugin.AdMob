using Google.Android.Libraries.Ads.Mobile.Sdk.Common;

namespace Plugin.AdMob.Platforms.Android.Native;

internal sealed class VideoLifecycleCallbacks : Java.Lang.Object, IVideoControllerVideoLifecycleCallbacks
{
    public event EventHandler? WhenVideoStarted;
    public event EventHandler? WhenVideoPlayed;
    public event EventHandler? WhenVideoPaused;
    public event EventHandler? WhenVideoEnded;
    public event EventHandler<bool>? WhenVideoMuted;

    public void OnVideoStart()
        => MainThreadDispatcher.Run(() => WhenVideoStarted?.Invoke(this, EventArgs.Empty));

    public void OnVideoPlay()
        => MainThreadDispatcher.Run(() => WhenVideoPlayed?.Invoke(this, EventArgs.Empty));

    public void OnVideoPause()
        => MainThreadDispatcher.Run(() => WhenVideoPaused?.Invoke(this, EventArgs.Empty));

    public void OnVideoEnd()
        => MainThreadDispatcher.Run(() => WhenVideoEnded?.Invoke(this, EventArgs.Empty));

    public void OnVideoMute(bool isMuted)
        => MainThreadDispatcher.Run(() => WhenVideoMuted?.Invoke(this, isMuted));
}
