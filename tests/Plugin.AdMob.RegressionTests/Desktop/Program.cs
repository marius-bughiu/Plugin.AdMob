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

IAdError legacyError = new MessageOnlyError("Legacy error");
if (legacyError.Message != "Legacy error" || legacyError.Code is not null || legacyError.Domain is not null || legacyError.ResponseId is not null)
{
    throw new Exception("A message-only implementation must keep the default error metadata.");
}

IAdError structuredError = new AdError("No fill", 3, "test-domain", "response-123");
if (structuredError.Message != "No fill" || structuredError.Code != 3
    || structuredError.Domain != "test-domain" || structuredError.ResponseId != "response-123")
{
    throw new Exception("The structured error did not preserve all its metadata.");
}

Console.WriteLine("PASS: structured ad errors preserve metadata and message-only implementations still work.");

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

    if (error.Code is not null || error.Domain is not null || error.ResponseId is not null)
        throw new Exception("An unsupported-platform error reported native SDK metadata.");

    var format = sender?.GetType() ?? throw new Exception("The failure callback did not identify its ad.");
    failures[format] = failures.GetValueOrDefault(format) + 1;
}

internal sealed class MessageOnlyError(string message) : IAdError
{
    public string Message { get; } = message;
}
