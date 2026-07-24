namespace Plugin.AdMob.DeviceTests;

/// <summary>
/// Writes harness results to the platform log under a stable tag so CI can scrape them.
/// The line CI keys off is: <c>SUMMARY status=PASS|FAIL passed=&lt;n&gt; total=&lt;m&gt;</c>.
/// </summary>
internal static class HarnessLog
{
    public const string Tag = "AdMobHarness";

    public static void Line(string message)
    {
#if ANDROID
        Android.Util.Log.Info(Tag, message);
#else
        Console.WriteLine($"{Tag}: {message}");
#endif
    }
}
