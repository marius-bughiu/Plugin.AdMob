using Plugin.AdMob.Services;

namespace Foo.Bar.SampleApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IInterstitialAdService _interstitialAdService;

        public MainPage()
        {
            InitializeComponent();

            _interstitialAdService = Services.ServiceProvider.GetService<IInterstitialAdService>();
            _interstitialAdService.PrepareAd();
        }

        private void OnShowInterstitialClicked(object sender, EventArgs e)
        {
            _interstitialAdService.ShowAd();
        }
    }

}
