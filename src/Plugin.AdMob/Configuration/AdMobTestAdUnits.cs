namespace Plugin.AdMob.Configuration;

/// <summary>
/// The quickest way to enable testing is to use Google-provided test ad units. These ad units are not associated with your AdMob account, 
/// so there's no risk of your account generating invalid traffic when using these ad units.
/// Source: https://developers.google.com/admob/android/test-ads
/// </summary>
internal static class AdMobTestAdUnits
{
    public static string Banner => "ca-app-pub-3940256099942544/6300978111";

    public static string Interstitial => "ca-app-pub-3940256099942544/1033173712";

    public static string InterstitialVideo => "ca-app-pub-3940256099942544/8691691433";

    public static string RewardedVideo => "ca-app-pub-3940256099942544/5224354917";
    
    public static string RewardedInterstitial => "ca-app-pub-3940256099942544/5354046379";

    public static string OpenApp => "ca-app-pub-3940256099942544/9257395921";

    public static string Native => "ca-app-pub-3940256099942544/2247696110";

    public static string NativeVideo => "ca-app-pub-3940256099942544/1044960115";
}
