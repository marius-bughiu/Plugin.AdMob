using Plugin.AdMob;
using Plugin.AdMob.Services;
using System.Diagnostics;
using ServiceProvider = Foo.Bar.SampleApp.Services.ServiceProvider;

namespace Foo.Bar.SampleApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IInterstitialAdService _interstitialAdService;
        private readonly IRewardedInterstitialAdService _rewardedInterstitialAdService;
        private readonly IRewardedAdService _rewardedAdService;
        private readonly IAppOpenAdService _appOpenAdService;
        private readonly IAdConsentService _adConsentService;

        public MainPage()
        {
            InitializeComponent();

            _interstitialAdService = ServiceProvider.GetRequiredService<IInterstitialAdService>();
            _rewardedAdService = ServiceProvider.GetRequiredService<IRewardedAdService>();
            _rewardedInterstitialAdService = ServiceProvider.GetRequiredService<IRewardedInterstitialAdService>();
            _appOpenAdService = ServiceProvider.GetRequiredService<IAppOpenAdService>();
            _adConsentService = ServiceProvider.GetRequiredService<IAdConsentService>();

            _interstitialAdService.PrepareAd();
            _rewardedAdService.PrepareAd(onUserEarnedReward: UserDidEarnReward);
            _rewardedInterstitialAdService.PrepareAd(onUserEarnedReward: UserDidEarnReward);
            _appOpenAdService.PrepareAd();

            _adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;
            UpdateCanRequestAds();
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

        private void OnShowRewardedAdClicked(object sender, EventArgs e)
        {
            _rewardedAdService.ShowAd();
            _rewardedAdService.PrepareAd(onUserEarnedReward: UserDidEarnReward);
        }

        private void OnCreateRewardedAdClicked(object sender, EventArgs e)
        {
            var rewardedAd = _rewardedAdService.CreateAd();
            rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;

            rewardedAd.OnUserEarnedReward += (_, reward) =>
            {
                UserDidEarnReward(reward);
            };
            rewardedAd.Load();
        }

        private void OnShowRewardedInterstitialClicked(object sender, EventArgs e)
        {
            _rewardedInterstitialAdService.ShowAd();
            _rewardedInterstitialAdService.PrepareAd(onUserEarnedReward: UserDidEarnReward);
        }

        private void OnCreateRewardedInterstitialClicked(object sender, EventArgs e)
        {
            var rewardedInterstitialAd = _rewardedInterstitialAdService.CreateAd();
            rewardedInterstitialAd.OnUserEarnedReward += (_, reward) =>
            {
                UserDidEarnReward(reward);
            };
            rewardedInterstitialAd.OnAdLoaded += RewardedInterstitialAd_OnAdLoaded;
            rewardedInterstitialAd.Load();
        }

        private void OnShowAppOpenClicked(object sender, EventArgs e)
        {
            _appOpenAdService.ShowAd();
            _appOpenAdService.PrepareAd();
        }

        private void OnCreateAppOpenClicked(object sender, EventArgs e)
        {
            var appOpenAd = _appOpenAdService.CreateAd();
            appOpenAd.OnAdLoaded += AppOpenAd_OnAdLoaded;
            appOpenAd.Load();
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

        private void RewardedAd_OnAdLoaded(object? sender, EventArgs e)
        {
            if (sender is IRewardedAd rewardedAd)
            {
                rewardedAd.Show();
            }
        }

        private void RewardedInterstitialAd_OnAdLoaded(object? sender, EventArgs e)
        {
            if (sender is IRewardedInterstitialAd rewardedInterstitialAd)
            {
                rewardedInterstitialAd.Show();
            }
        }

        private static void UserDidEarnReward(RewardItem rewardItem)
        {
            Debug.WriteLine($"User earned {rewardItem.Amount} {rewardItem.Type}.");
        }

        private void AppOpenAd_OnAdLoaded(object? sender, EventArgs e)
        {
            if (sender is IAppOpenAd appOpenAd)
            {
                appOpenAd.Show();
            }
        }
    }

}
