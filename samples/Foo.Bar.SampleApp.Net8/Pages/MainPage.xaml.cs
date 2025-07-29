using Foo.Bar.SampleApp.Pages;
using Foo.Bar.SampleApp.ViewModels;
using Foo.Bar.SampleApp.Views;
using Plugin.AdMob;
using Plugin.AdMob.Services;
using System.Diagnostics;
using ServiceProvider = Foo.Bar.SampleApp.Services.ServiceProvider;

namespace Foo.Bar.SampleApp.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly IInterstitialAdService _interstitialAdService;
        private readonly IAdConsentService _adConsentService;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            _interstitialAdService = ServiceProvider.GetRequiredService<IInterstitialAdService>();
            _adConsentService = ServiceProvider.GetRequiredService<IAdConsentService>();

            _interstitialAdService.OnAdLoaded += (_, __) => Debug.WriteLine("Interstitial ad prepared.");
            _interstitialAdService.PrepareAd();

            _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
            UpdateCanRequestAds();
        }

        private void OnShowInterstitialClicked(object sender, EventArgs e)
        {
            if (_interstitialAdService.IsAdLoaded)
            {
                _interstitialAdService.ShowAd();
            }

            _interstitialAdService.PrepareAd();
        }

        private void OnShowIfRequiredClicked(object sender, EventArgs e)
        {
            _adConsentService.LoadAndShowConsentFormIfRequired();
        }

        private void OnShowPrivacyOptionsClicked(object sender, EventArgs e)
        {
            _adConsentService.ShowPrivacyOptionsForm();
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            _adConsentService.Reset();
        }

        private void UpdateCanRequestAds()
        {
            CanRequestAdsLabel.Text = _adConsentService.CanRequestAds().ToString();
        }

        private void OnConsentInfoUpdated(object? sender, IConsentInformation? e)
        {
            UpdateCanRequestAds();
        }

        private void InterstitialAd_OnAdLoaded(object? sender, EventArgs e)
        {
            if (sender is IInterstitialAd interstitialAd)
            {
                interstitialAd.Show();
            }
        }

        private void BannerAd_OnAdLoaded(object sender, EventArgs e)
        {
            Debug.WriteLine("Banner ad loaded.");
        }

        private void NativeAdView_OnAdLoaded(object sender, EventArgs e)
        {
            Debug.WriteLine("Native ad loaded.");
        }

        private void NativeAdView_OnAdImpression(object sender, EventArgs e)
        {
            Debug.WriteLine("Native ad displayed.");
        }

        private void NativeAdView_OnAdClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Native ad clicked.");
        }

        private void NativeAdView_OnAdOpened(object sender, EventArgs e)
        {
            Debug.WriteLine("Native ad opened.");
        }

        private void NativeAdView_OnAdSwiped(object sender, EventArgs e)
        {
            Debug.WriteLine("Native ad swiped.");
        }

        private void NativeAdView_OnAdClosed(object sender, EventArgs e)
        {
            Debug.WriteLine("Native ad closed.");
        }
    }

}
