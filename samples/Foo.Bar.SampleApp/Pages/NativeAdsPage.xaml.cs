using Foo.Bar.SampleApp.Views;
using Plugin.AdMob;
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
        // Google's native VIDEO demo ad unit. UseTestAdUnitIds overrides any explicit
        // ad unit with the (image-only) native demo unit, so it is flipped off around
        // CreateAd; the demo unit below is itself a Google test unit, so this is safe.
        var useTestAdUnitIds = Plugin.AdMob.Configuration.AdConfig.UseTestAdUnitIds;
        Plugin.AdMob.Configuration.AdConfig.UseTestAdUnitIds = false;
        var nativeAd = _nativeAdService.CreateAd("ca-app-pub-3940256099942544/1044960115");
        Plugin.AdMob.Configuration.AdConfig.UseTestAdUnitIds = useTestAdUnitIds;

        nativeAd.OnAdLoaded += (_, _) =>
        {
            var nativeAdView = new NativeAdView(nativeAd, BuildVideoAdTemplate());
            this.LayoutRoot.Add(nativeAdView);
        };
        nativeAd.OnAdFailedToLoad += (_, error) =>
        {
            System.Diagnostics.Debug.WriteLine($"Native video ad failed to load: {error.Message}");
        };

        nativeAd.Load();
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