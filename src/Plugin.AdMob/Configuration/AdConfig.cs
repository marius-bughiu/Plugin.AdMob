namespace Plugin.AdMob.Configuration;

public static class AdConfig
{
    public static string DefaultBannerAdUnitId { get; set; }

    public static string DefaultInterstitialAdUnitId { get; set; }
    
    public static string DefaultRewardedAdUnitId { get; set; }

    public static IList<string> TestDevices { get; } = new List<string>();

    public static bool UseTestAdUnitIds { get; set; }

    public static void AddTestDevice(string deviceId)
    {
        TestDevices.Add(deviceId);
    }
}
