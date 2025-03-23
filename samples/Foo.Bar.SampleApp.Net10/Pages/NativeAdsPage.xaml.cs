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
}