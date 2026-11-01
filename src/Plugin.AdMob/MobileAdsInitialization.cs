#if ANDROID || IOS
#if ANDROID
using Plugin.AdMob.Platforms.Android;
#else
using Plugin.AdMob.Platforms.iOS;
#endif

namespace Plugin.AdMob;

internal static class MobileAdsInitialization
{
    private static int _started;

    internal static void Start()
    {
        if (Interlocked.Exchange(ref _started, 1) == 1)
        {
            return;
        }

#if ANDROID
        AdMobInitializer.EnsureInitialized();
#else
        Google.MobileAds.MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
        Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);
#endif
    }
}
#endif
