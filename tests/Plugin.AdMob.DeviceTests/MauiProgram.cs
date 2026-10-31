using Microsoft.Maui.Hosting;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.DeviceTests;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Route every request to Google's official test ad units — no AdMob account,
        // no invalid-traffic risk, and Google guarantees these demo units fill.
        AdConfig.UseTestAdUnitIds = true;

        var builder = MauiApp.CreateBuilder();

        // The plugin's defaults, as a shipped app runs them: automatic consent on activation and
        // the consent gate before every load. The "Other" debug geography resolves the update
        // without a consent form, so the run stays headless.
        builder
            .UseMauiApp<App>()
            .UseAdMob()
            .UseConsentDebugSettings(new ConsentDebugSettings
            {
                Geography = ConsentDebugGeography.Other,
                TestDeviceHashedIds = [.. HarnessConsent.TestDeviceHashedIds()],
                Reset = true,
            });

        var app = builder.Build();
        HarnessConsent.Observe(app.Services.GetRequiredService<IAdConsentService>());
        EarlyLoads.Start(app.Services);

        return app;
    }
}
