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
                    //Reset = true
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
