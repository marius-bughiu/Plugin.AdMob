using Plugin.AdMob;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

var consent = new AdConsentService();
if (consent.CanRequestAds())
    throw new Exception("Desktop consent unexpectedly allows ads.");
var interstitial = new InterstitialAdService(consent);
var rewarded = new RewardedAdService(consent);
var rewardedInterstitial = new RewardedInterstitialAdService(consent);
var appOpen = new AppOpenAdService(consent);
var failures = new Dictionary<Type, int>();
Type[] formats = [typeof(InterstitialAd), typeof(RewardedAd), typeof(RewardedInterstitialAd), typeof(AppOpenAd)];
interstitial.OnAdFailedToLoad += RecordFailure;
rewarded.OnAdFailedToLoad += RecordFailure;
rewardedInterstitial.OnAdFailedToLoad += RecordFailure;
appOpen.OnAdFailedToLoad += RecordFailure;
PrepareAll();
Check("PrepareAd() with default settings");

var interstitialAd = interstitial.CreateAd();
var rewardedAd = rewarded.CreateAd();
var rewardedInterstitialAd = rewardedInterstitial.CreateAd();
var appOpenAd = appOpen.CreateAd();
interstitialAd.OnAdFailedToLoad += RecordFailure;
rewardedAd.OnAdFailedToLoad += RecordFailure;
rewardedInterstitialAd.OnAdFailedToLoad += RecordFailure;
appOpenAd.OnAdFailedToLoad += RecordFailure;
interstitialAd.Load();
rewardedAd.Load();
rewardedInterstitialAd.Load();
appOpenAd.Load();
Check("CreateAd().Load()");

AdConfig.DisableConsentCheck = true;
PrepareAll();
Check("PrepareAd() with consent checks disabled");

void Check(string scenario)
{
    foreach (var format in formats)
    {
        var count = failures.GetValueOrDefault(format);
        if (count != 1)
        {
            throw new Exception($"{scenario} expected one {format.Name} error, got {count}.");
        }
    }

    Console.WriteLine($"PASS: {scenario} reported one unsupported-platform error for each full-screen ad format.");
    failures.Clear();
}

void PrepareAll()
{
    interstitial.PrepareAd();
    rewarded.PrepareAd();
    rewardedInterstitial.PrepareAd();
    appOpen.PrepareAd();
    if (interstitial.IsAdLoaded || rewarded.IsAdLoaded || rewardedInterstitial.IsAdLoaded || appOpen.IsAdLoaded)
        throw new Exception("An ad was incorrectly marked as loaded.");
}

void RecordFailure(object? sender, IAdError error)
{
    if (error.Message != AdError.UnsupportedPlatformMessage)
        throw new Exception(error.Message);

    var format = sender?.GetType() ?? throw new Exception("The failure callback did not identify its ad.");
    failures[format] = failures.GetValueOrDefault(format) + 1;
}
