using Foo.Bar.SampleApp.Pages;
using Foo.Bar.SampleApp.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;

namespace Foo.Bar.SampleApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            AdConfig.UseTestAdUnitIds = true;

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseAdMob()
                .UseConsentDebugSettings(new ConsentDebugSettings
                {
                    Geography = ConsentDebugGeography.Eea,
                    TestDeviceHashedIds = ["33BE2250B43518CCDA7DE426D04EE231"],
                    // Reset = true
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<BannerAdsViewModel>();
            builder.Services.AddTransient<InterstitialAdsViewModel>();
            builder.Services.AddTransient<NativeAdsViewModel>();
            builder.Services.AddTransient<AdTargetingOptionsViewModel>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<BannerAdsPage>();
            builder.Services.AddTransient<InterstitialAdsPage>();
            builder.Services.AddTransient<NativeAdsPage>();
            builder.Services.AddTransient<AdTargetingOptionsPage>();

            return builder.Build();
        }
    }
}
