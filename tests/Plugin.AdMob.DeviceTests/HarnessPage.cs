namespace Plugin.AdMob.DeviceTests;

/// <summary>
/// The app's only page. On first render it kicks off the ad-load harness; the banner
/// format needs a live view in the tree, so the harness is handed this page's banner host.
/// </summary>
public class HarnessPage : ContentPage
{
    private readonly VerticalStackLayout _bannerHost = new();
    private bool _started;

    public HarnessPage()
    {
        Title = "AdMob Device Tests";
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Spacing = 8,
                Children =
                {
                    new Label { Text = "AdMob device test harness running…" },
                    _bannerHost,
                },
            },
        };

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (_started)
        {
            return;
        }

        _started = true;

        try
        {
            await AdLoadHarness.RunAllAsync(_bannerHost);
        }
        catch (Exception ex)
        {
            // Emit the summary lines CI scrapes so a startup failure fails the run fast
            // instead of waiting out the log-polling timeout.
            HarnessLog.Line($"RESULT format=harness status=FAIL detail=\"{ex.Message.Replace('\n', ' ').Replace('\r', ' ').Replace('"', '\'')}\"");
            HarnessLog.Line("SUMMARY_BANNER status=FAIL");
            HarnessLog.Line("SUMMARY_ALL status=FAIL passed=0 total=0");
        }
    }
}
