using Foo.Bar.SampleApp.Views;
using Plugin.AdMob;
using Plugin.AdMob.Services;
using ServiceProvider = Foo.Bar.SampleApp.Services.ServiceProvider;

namespace Foo.Bar.SampleApp.Pages;

public partial class NativeAdsPage : ContentPage
{
    private readonly INativeAdService _nativeAdService;
    private readonly INativeVideoAdService _nativeVideoAdService;

    public NativeAdsPage()
	{
		InitializeComponent();

        _nativeAdService = ServiceProvider.GetRequiredService<INativeAdService>();
        _nativeVideoAdService = ServiceProvider.GetRequiredService<INativeVideoAdService>();
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
        var nativeVideoAd = _nativeVideoAdService.CreateAd();
        nativeVideoAd.OnAdLoaded += (_, _) =>
        {
            if (nativeVideoAd.HasVideoContent)
            {
                DisplayAlert("Native Video Ad", 
                    $"Video duration: {nativeVideoAd.VideoDuration:F1}s\nAspect ratio: {nativeVideoAd.VideoAspectRatio:F2}", 
                    "OK");
            }

            var customAdView = new MyCustomAdTemplate();
            var nativeAdView = new NativeAdView(nativeVideoAd, customAdView);
            this.LayoutRoot.Add(nativeAdView);
        };

        nativeVideoAd.Load();
    }
}