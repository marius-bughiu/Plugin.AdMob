namespace Plugin.AdMob.Platforms.Android;

/// <summary>
/// The Google Mobile Ads Next-Gen SDK invokes its callbacks on a background thread, unlike the legacy SDK
/// which used the main thread. Plugin events end up in consumer code that updates the UI (and the banner and
/// native ad handlers touch the view hierarchy directly), so every callback is marshalled back to the main
/// thread before it is surfaced.
/// </summary>
internal static class MainThreadDispatcher
{
    internal static void Run(Action action)
    {
        if (MainThread.IsMainThread)
        {
            action();
            return;
        }

        MainThread.BeginInvokeOnMainThread(action);
    }
}
