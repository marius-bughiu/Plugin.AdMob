using Microsoft.Extensions.Logging;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;

namespace Foo.Bar.SampleApp.Hybrid
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
