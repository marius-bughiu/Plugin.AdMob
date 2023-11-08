using Microsoft.Maui.Controls;

namespace Plugin.AdMob;

public class BannerAd : ContentView
{
    public static readonly BindableProperty AdUnitIdProperty =
        BindableProperty.Create("AdUnitId", typeof(string), typeof(BannerAd), null);

    public string AdUnitId
    {
        get { return (string)GetValue(AdUnitIdProperty); }
        set { SetValue(AdUnitIdProperty, value); }
    }

    public static readonly BindableProperty AdSizeProperty =
        BindableProperty.Create("AdSize", typeof(AdSize), typeof(BannerAd), null);

    public AdSize? AdSize
    {
        get { return (AdSize?)GetValue(AdSizeProperty); }
        set { SetValue(AdSizeProperty, value); }
    }

    public static readonly BindableProperty AdWidthProperty =
        BindableProperty.Create("AdWidth", typeof(int), typeof(BannerAd), null);

    public int CustomAdWidth
    {
        get { return (int)GetValue(AdWidthProperty); }
        set { SetValue(AdWidthProperty, value); }
    }

    public static readonly BindableProperty AdHeightProperty =
        BindableProperty.Create("AdHeight", typeof(int), typeof(BannerAd), null);

    public int CustomAdHeight
    {
        get { return (int)GetValue(AdHeightProperty); }
        set { SetValue(AdHeightProperty, value); }
    }
}
