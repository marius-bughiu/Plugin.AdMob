using Foo.Bar.SampleApp.Views;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;
using ServiceProvider = Foo.Bar.SampleApp.Services.ServiceProvider;

namespace Foo.Bar.SampleApp.Pages;

public partial class NativeAdsPage : ContentPage
{
    private readonly INativeAdService _nativeAdService;

    public NativeAdsPage()
	{
		InitializeComponent();

        _nativeAdService = ServiceProvider.GetRequiredService<INativeAdService>();
    }

    private void OnLoadNativeAdClicked(object sender, EventArgs e)
    {
        var nativeAd = _nativeAdService.CreateAd();
        nativeAd.OnAdLoaded += (_, _) =>
        {
            var customAdView = new MyCustomAdTemplate();
            var nativeAdView = new NativeAdView(nativeAd, customAdView);
            this.LayoutRoot.Add(nativeAdView);
        };

        nativeAd.Load();
    }

    private void OnLoadNativeVideoAdClicked(object sender, EventArgs e)
    {
        var nativeAd = _nativeAdService.CreateAd(
            videoOptions: new VideoOptions { StartMuted = true });

        nativeAd.OnAdLoaded += (_, _) =>
        {
            var nativeAdView = new NativeAdView(nativeAd, BuildVideoAdTemplate());
            this.LayoutRoot.Add(nativeAdView);
        };
        nativeAd.OnAdFailedToLoad += (_, error) =>
        {
            System.Diagnostics.Debug.WriteLine($"Native video ad failed to load: {error.Message}");
        };

        nativeAd.OnVideoStart += (_, _) => LogVideoEvent("start");
        nativeAd.OnVideoPlay += (_, _) => LogVideoEvent($"play, duration: {nativeAd.VideoDuration:mm\\:ss}");
        nativeAd.OnVideoPause += (_, _) => LogVideoEvent("pause");
        nativeAd.OnVideoEnd += (_, _) => LogVideoEvent("end");
        nativeAd.OnVideoMuted += (_, isMuted) => LogVideoEvent(isMuted ? "muted" : "unmuted");

        nativeAd.Load();
    }

    private static void LogVideoEvent(string name)
    {
        System.Diagnostics.Debug.WriteLine($"Native video event: {name}.");
    }

    private static ContentView BuildVideoAdTemplate()
    {
        var headline = new Label { FontAttributes = FontAttributes.Bold, FontSize = 16 };
        headline.SetBinding(Label.TextProperty, nameof(INativeAd.Headline));

        var body = new Label { FontSize = 13 };
        body.SetBinding(Label.TextProperty, nameof(INativeAd.Body));

        return new ContentView
        {
            WidthRequest = 340,
            HeightRequest = 280,
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new MediaView { HeightRequest = 200, WidthRequest = 340 },
                    headline,
                    body,
                },
            },
        };
    }
}