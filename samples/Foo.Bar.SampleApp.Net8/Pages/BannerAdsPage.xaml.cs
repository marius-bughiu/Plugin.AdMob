using Foo.Bar.SampleApp.ViewModels;
using System.Diagnostics;

namespace Foo.Bar.SampleApp.Pages;

public partial class BannerAdsPage : ContentPage
{
	public BannerAdsPage(BannerAdsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private void BannerAd_OnAdLoaded(object sender, EventArgs e)
    {
        Debug.WriteLine("Banner ad loaded.");
    }
}