using Google.Android.Libraries.Ads.Mobile.Sdk.Common;

namespace Plugin.AdMob.Platforms.Android;

/// <summary>
/// Bridges the next-gen <see cref="IAdLoadCallback" /> to plain C# events. The loaded ad is delivered as a
/// <see cref="Java.Lang.Object" />; callers use <c>JavaCast&lt;T&gt;()</c> to obtain the concrete ad interface
/// for their format.
/// </summary>
internal sealed class AdLoadCallback : Java.Lang.Object, IAdLoadCallback
{
    public event EventHandler<Java.Lang.Object>? Loaded;
    public event EventHandler<LoadAdError>? Failed;

    public void OnAdLoaded(Java.Lang.Object? ad)
    {
        if (ad is not null)
        {
            MainThreadDispatcher.Run(() => Loaded?.Invoke(this, ad));
        }
    }

    public void OnAdFailedToLoad(LoadAdError adError)
        => MainThreadDispatcher.Run(() => Failed?.Invoke(this, adError));
}
