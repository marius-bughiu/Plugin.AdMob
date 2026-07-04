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
        // Google's native VIDEO demo ad unit. An explicitly passed ad unit ID wins over
        // AdConfig.UseTestAdUnitIds, which would otherwise route to the image-only native
        // demo unit.
        var nativeAd = _nativeAdService.CreateAd(
            "ca-app-pub-3940256099942544/1044960115",
            new VideoOptions { StartMuted = true });

        var status = new Label { FontSize = 12 };

        nativeAd.OnAdLoaded += (_, _) =>
        {
            var nativeAdView = new NativeAdView(nativeAd, BuildVideoAdTemplate());
            this.LayoutRoot.Add(nativeAdView);
            this.LayoutRoot.Add(status);

            status.Text = $"HasVideoContent: {nativeAd.HasVideoContent}, " +
                $"aspect: {nativeAd.VideoAspectRatio:0.##}, duration: {nativeAd.VideoDuration:mm\\:ss}";
        };
        nativeAd.OnAdFailedToLoad += (_, error) =>
        {
            System.Diagnostics.Debug.WriteLine($"Native video ad failed to load: {error.Message}");
        };

        nativeAd.OnVideoStart += (_, _) => AppendVideoEvent(status, "start");
        nativeAd.OnVideoPlay += (_, _) => AppendVideoEvent(status, "play");
        nativeAd.OnVideoPause += (_, _) => AppendVideoEvent(status, "pause");
        nativeAd.OnVideoEnd += (_, _) => AppendVideoEvent(status, "end");
        nativeAd.OnVideoMuted += (_, isMuted) => AppendVideoEvent(status, isMuted ? "muted" : "unmuted");

        nativeAd.Load();
    }

    private static void AppendVideoEvent(Label status, string name)
    {
        System.Diagnostics.Debug.WriteLine($"Native video event: {name}.");
        MainThread.BeginInvokeOnMainThread(() => status.Text += $" [{name}]");
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