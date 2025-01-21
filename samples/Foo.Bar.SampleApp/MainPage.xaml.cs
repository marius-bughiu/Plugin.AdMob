using Plugin.AdMob;
using Plugin.AdMob.Services;
using System.Diagnostics;

namespace Foo.Bar.SampleApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IInterstitialAdService _interstitialAdService;
        private readonly IRewardedAdService _rewardedAdService;

        public MainPage()
        {
            InitializeComponent();

            _interstitialAdService = Services.ServiceProvider.GetService<IInterstitialAdService>();
            _interstitialAdService.PrepareAd();

            _rewardedAdService = Services.ServiceProvider.GetService<IRewardedAdService>();
            _rewardedAdService.PrepareAd();
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

        private void OnCreateRewardedAdClicked(object sender, EventArgs e)
        {
            var rewardedAd = _rewardedAdService.CreateAd();
            rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;
            
            rewardedAd.OnUserEarnedReward += (s, reward) =>
            {
                Debug.WriteLine($"User earned {reward.Amount} {reward.Type}.");
            };
            rewardedAd.Load();
        }

        private void InterstitialAd_OnAdLoaded(object sender, EventArgs e)
        {
            (sender as IInterstitialAd).Show();
        }

        private void BannerAd_OnAdLoaded(object sender, EventArgs e)
        {
            Debug.WriteLine("Banner ad loaded.");
        }
        
        private void RewardedAd_OnAdLoaded(object sender, EventArgs e)
        {
            (sender as IRewardedAd).Show();
        }
    }

}
