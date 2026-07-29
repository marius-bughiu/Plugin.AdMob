using Google.Android.Libraries.Ads.Mobile.Sdk.Rewarded;

namespace Plugin.AdMob.Platforms.Android;

/// <summary>
/// Bridges the next-gen <see cref="IOnUserEarnedRewardListener" /> to a plain C# event.
/// </summary>
internal sealed class UserEarnedRewardListener : Java.Lang.Object, IOnUserEarnedRewardListener
{
    public event EventHandler<IRewardItem>? UserEarnedReward;

    public void OnUserEarnedReward(IRewardItem reward)
        => MainThreadDispatcher.Run(() => UserEarnedReward?.Invoke(this, reward));
}
