using Foo.Bar.SampleApp.ViewModels;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;

namespace Foo.Bar.SampleApp.Pages;

public partial class AdTargetingOptionsPage : ContentPage
{
    public AdTargetingOptionsPage(AdTargetingOptionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TagForChildDirectedTreatmentPicker.SelectedItem = AdConfig.TagForChildDirectedTreatment;
        TagForUnderAgeOfConsentPicker.SelectedItem = AdConfig.TagForUnderAgeOfConsent;
        MaxAdContentRatingPicker.SelectedItem = AdConfig.MaxAdContentRating;
    }

    protected override void OnDisappearing()
    {
        AdConfig.TagForChildDirectedTreatment = (TagForChildDirectedTreatment)TagForChildDirectedTreatmentPicker.SelectedItem;
        AdConfig.TagForUnderAgeOfConsent = (TagForUnderAgeOfConsent)TagForUnderAgeOfConsentPicker.SelectedItem;
        AdConfig.MaxAdContentRating = (MaxAdContentRating)MaxAdContentRatingPicker.SelectedItem;

        base.OnDisappearing();
    }
}