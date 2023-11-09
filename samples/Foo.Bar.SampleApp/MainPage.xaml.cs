using Plugin.AdMob.Services;

namespace Foo.Bar.SampleApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IInterstitialAdService _interstitialAdService;

        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            _interstitialAdService = Services.ServiceProvider.GetService<IInterstitialAdService>();
            _interstitialAdService.PrepareAd();
        }

        private void OnShowInterstitialClicked(object sender, EventArgs e)
        {
            _interstitialAdService.ShowAd();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
