using Foo.Bar.SampleApp.Pages;

namespace Foo.Bar.SampleApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(BannerAdsPage), typeof(BannerAdsPage));
            Routing.RegisterRoute(nameof(InterstitialAdsPage), typeof(InterstitialAdsPage));
            Routing.RegisterRoute(nameof(NativeAdsPage), typeof(NativeAdsPage));
            Routing.RegisterRoute(nameof(AdTargetingOptionsPage), typeof(AdTargetingOptionsPage));
        }
    }
}
