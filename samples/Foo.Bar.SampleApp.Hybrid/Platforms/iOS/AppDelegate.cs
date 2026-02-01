using Foundation;

namespace Foo.Bar.SampleApp.Hybrid
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            var app = MauiProgram.CreateMauiApp();

            // Initialize Google Mobile Ads SDK
            // Note: SharedInstance may be null if the native SDK binding failed to load
            var sharedInstance = Google.MobileAds.MobileAds.SharedInstance;
            if (sharedInstance is null)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: Google.MobileAds.MobileAds.SharedInstance is null. " +
                    "The Google Mobile Ads SDK may not be properly initialized. " +
                    "Check that GADApplicationIdentifier is set in Info.plist and the Jc.GMA.iOS package is properly referenced.");
            }
            else
            {
                sharedInstance.Start(completionHandler: null);
            }

            return app;
        }
    }
}
