using Plugin.AdMob;
using Plugin.AdMob.Services;
using System.Diagnostics;

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
            _interstitialAdService.PrepareAd();
        }

        private void OnCreateInterstitialClicked(object sender, EventArgs e)
        {
            var interstitialAd = _interstitialAdService.CreateAd();
            interstitialAd.OnAdLoaded += InterstitialAd_OnAdLoaded;
            interstitialAd.Load();
        }

        private void InterstitialAd_OnAdLoaded(object sender, EventArgs e)
        {
            (sender as IInterstitialAd).Show();
        }

        private void BannerAd_OnAdLoaded(object sender, EventArgs e)
        {
            Debug.WriteLine("Banner ad loaded.");
        }
    }

}
