using Microsoft.Maui.Hosting;
using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.DeviceTests;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Route every request to Google's official test ad units — no AdMob account,
        // no invalid-traffic risk, and Google guarantees these demo units fill.
        AdConfig.UseTestAdUnitIds = true;

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            // Headless run: bypass the UMP consent gate (a fresh app has no consent info,
            // which would otherwise make every load early-return) and never surface a
            // consent form.
            .UseAdMob(disableConsentCheck: true, automaticallyAskForConsent: false);

        return builder.Build();
    }
}
