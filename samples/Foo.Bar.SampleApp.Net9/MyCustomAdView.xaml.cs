using Plugin.AdMob;

namespace Foo.Bar.SampleApp.Net9;

public partial class MyCustomAdView : ContentView
{
	public MyCustomAdView(INativeAd nativeAd)
	{
		InitializeComponent();

		AdImage.Source = nativeAd.Image;
	}
}