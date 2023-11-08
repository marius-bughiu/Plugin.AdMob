using Plugin.AdMob.Handlers;

namespace Plugin.AdMob;

public static class Config
{
    public static MauiAppBuilder UseAdMobHandlers(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(BannerAd), typeof(BannerAdHandler));
        });

        return builder;
    }
}
