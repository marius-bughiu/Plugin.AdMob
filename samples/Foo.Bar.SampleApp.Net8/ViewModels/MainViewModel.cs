using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foo.Bar.SampleApp.Pages;

namespace Foo.Bar.SampleApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    Task NavigateToBannerAds() => Shell.Current.GoToAsync(nameof(BannerAdsPage));

    [RelayCommand]
    Task NavigateToInterstitialAds() => Shell.Current.GoToAsync(nameof(InterstitialAdsPage));

    [RelayCommand]
    Task NavigateToNativeAds() => Shell.Current.GoToAsync(nameof(NativeAdsPage));

    [RelayCommand]
    Task NavigateToAdTargetingOptions() => Shell.Current.GoToAsync(nameof(AdTargetingOptionsPage));
}
