#if !ANDROID && !IOS
namespace Plugin.AdMob.Services
{
    internal partial class AdConsentService
    {
        public void LoadAndShowConsentFormIfRequired() { }

        public bool CanRequestAds() => false;

        public bool IsPrivacyOptionsRequired() => false;

        public void ShowPrivacyOptionsForm() { }

        public void Reset() { }
    }
}

namespace Plugin.AdMob
{
    internal partial class InterstitialAd
    {
        public void Load() => OnAdFailedToLoad?.Invoke(this, new AdError(AdError.UnsupportedPlatformMessage));

        public void Show() { }
    }

    internal partial class RewardedAd
    {
        public void Load() => OnAdFailedToLoad?.Invoke(this, new AdError(AdError.UnsupportedPlatformMessage));

        public void Show() { }
    }

    internal partial class RewardedInterstitialAd
    {
        public void Load() => OnAdFailedToLoad?.Invoke(this, new AdError(AdError.UnsupportedPlatformMessage));

        public void Show() { }
    }

    internal partial class AppOpenAd
    {
        public void Load() => OnAdFailedToLoad?.Invoke(this, new AdError(AdError.UnsupportedPlatformMessage));

        public void Show() { }
    }

    internal partial class NativeAd
    {
        public string? Advertiser => null;

        public string? Body => null;

        public string? CallToAction => null;

        public string? Headline => null;

        public string? IconUri => null;

        public string? ImageUri => null;

        public string? Price => null;

        public double? StarRating => null;

        public string? Store => null;

        public bool HasVideoContent => false;

        public double VideoAspectRatio => 0;

        public TimeSpan VideoDuration => TimeSpan.Zero;

        public TimeSpan VideoCurrentTime => TimeSpan.Zero;

        public bool IsVideoMuted => false;

        public bool VideoCustomControlsEnabled => false;

        public bool VideoClickToExpandEnabled => false;

        public void PlayVideo() { }

        public void PauseVideo() { }

        public void SetVideoMuted(bool muted) { }

        public void Load() => OnAdFailedToLoad?.Invoke(this, new AdError(AdError.UnsupportedPlatformMessage));
    }
}
#endif
