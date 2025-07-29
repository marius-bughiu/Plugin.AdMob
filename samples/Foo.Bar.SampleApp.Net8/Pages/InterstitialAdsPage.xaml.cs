using Foo.Bar.SampleApp.ViewModels;
using Plugin.AdMob;
using Plugin.AdMob.Services;
using System.Diagnostics;
using ServiceProvider = Foo.Bar.SampleApp.Services.ServiceProvider;

namespace Foo.Bar.SampleApp.Pages;

public partial class InterstitialAdsPage : ContentPage
{
    private readonly IInterstitialAdService _interstitialAdService;
    private readonly IRewardedInterstitialAdService _rewardedInterstitialAdService;
    private readonly IRewardedAdService _rewardedAdService;
    private readonly IAppOpenAdService _appOpenAdService;

    public InterstitialAdsPage(InterstitialAdsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        _interstitialAdService = ServiceProvider.GetRequiredService<IInterstitialAdService>();
        _rewardedAdService = ServiceProvider.GetRequiredService<IRewardedAdService>();
        _rewardedInterstitialAdService = ServiceProvider.GetRequiredService<IRewardedInterstitialAdService>();
        _appOpenAdService = ServiceProvider.GetRequiredService<IAppOpenAdService>();

        _interstitialAdService.OnAdLoaded += (_, __) => Debug.WriteLine("Interstitial ad prepared.");
        _interstitialAdService.PrepareAd();

        _rewardedAdService.OnAdLoaded += (_, __) => Debug.WriteLine("Rewarded ad prepared.");
        _rewardedAdService.PrepareAd(onUserEarnedReward: UserDidEarnReward);

        _rewardedInterstitialAdService.OnAdLoaded += (_, __) => Debug.WriteLine("Rewarded interstitial ad prepared.");
        _rewardedInterstitialAdService.PrepareAd(onUserEarnedReward: UserDidEarnReward);

        _appOpenAdService.OnAdLoaded += (_, __) => Debug.WriteLine("App open ad prepared.");
        _appOpenAdService.PrepareAd();
    }

    private void OnShowInterstitialClicked(object sender, EventArgs e)
    {
        if (_interstitialAdService.IsAdLoaded)
        {
            _interstitialAdService.ShowAd();
        }

        _interstitialAdService.PrepareAd();
    }

    private void OnCreateInterstitialClicked(object sender, EventArgs e)
    {
        var interstitialAd = _interstitialAdService.CreateAd();
        interstitialAd.OnAdLoaded += InterstitialAd_OnAdLoaded;
        interstitialAd.Load();
    }

    private void InterstitialAd_OnAdLoaded(object? sender, EventArgs e)
    {
        if (sender is IInterstitialAd interstitialAd)
        {
            interstitialAd.Show();
        }
    }

    private void OnShowRewardedAdClicked(object sender, EventArgs e)
    {
        if (_rewardedAdService.IsAdLoaded)
        {
            _rewardedAdService.ShowAd();
        }

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
        if (_rewardedInterstitialAdService.IsAdLoaded)
        {
            _rewardedInterstitialAdService.ShowAd();
        }

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
        if (_appOpenAdService.IsAdLoaded)
        {
            _appOpenAdService.ShowAd();
        }

        _appOpenAdService.PrepareAd();
    }

    private void OnCreateAppOpenClicked(object sender, EventArgs e)
    {
        var appOpenAd = _appOpenAdService.CreateAd();
        appOpenAd.OnAdLoaded += AppOpenAd_OnAdLoaded;
        appOpenAd.OnAdImpression += AppOpenAd_OnAdImpression;
        appOpenAd.OnAdDismissed += AppOpenAd_OnAdDismissed;
        appOpenAd.Load();
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
        Debug.WriteLine("Open app ad loaded.");
        if (sender is IAppOpenAd appOpenAd)
        {
            appOpenAd.Show();
        }
    }

    private void AppOpenAd_OnAdImpression(object? sender, EventArgs e)
    {
        Debug.WriteLine("Open app ad displayed.");
    }

    private void AppOpenAd_OnAdDismissed(object? sender, EventArgs e)
    {
        Debug.WriteLine("Open app ad dismissed.");
    }
}