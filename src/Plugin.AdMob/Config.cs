using Plugin.AdMob.Handlers;
using Plugin.AdMob.Services;

namespace Plugin.AdMob;

public static class Config
{
    public static MauiAppBuilder UseAdMob(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(BannerAd), typeof(BannerAdHandler));
        });

        builder.Services.AddSingleton<IInterstitialAdService, InterstitialAdService>();

        return builder;
    }
}
