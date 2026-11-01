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
        var configBuilder = new Android.Gms.Ads.RequestConfiguration.Builder();
        configBuilder.ApplyGlobalAdConfiguration();
        Android.Gms.Ads.MobileAds.RequestConfiguration = configBuilder.Build();

        var context = Android.App.Application.Context;
        Task.Run(() => Android.Gms.Ads.MobileAds.Initialize(context));
#else
        Google.MobileAds.MobileAds.SharedInstance.RequestConfiguration.ApplyGlobalAdConfiguration();
        Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);
#endif
    }
}
#endif
