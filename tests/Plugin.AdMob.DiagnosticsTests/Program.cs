using System.Runtime.CompilerServices;
using Google.MobileAds;
using Plugin.AdMob;
using Plugin.AdMob.Platforms.iOS;

var (weakAd, retainedBySdk) = LoadUnshownAd();
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
Check(!weakAd.TryGetTarget(out _), "An SDK-owned paid handler retained an unshown ad.");
retainedBySdk(new AdValue());
Console.WriteLine("PASS: The paid handler can outlive a collected ad with no subscribers.");

var liveAd = new TestAd();
var paidHandler = PaidEventHandlers.Create(liveAd, static (ad, value) => ad.ReportPaid(value));
var paidValue = new AdValue();
var paidCount = 0;
void OnPaid(AdValue value)
{
    Check(ReferenceEquals(value, paidValue), "The paid payload changed in transit.");
    paidCount++;
}

paidHandler(paidValue);
liveAd.Paid += OnPaid;
paidHandler(paidValue);
liveAd.Paid -= OnPaid;
paidHandler(paidValue);
Check(paidCount == 1, "The handler failed to respect the current subscription.");
GC.KeepAlive(liveAd);
Console.WriteLine("PASS: A live ad delivers to the current subscriber once.");

IAdError legacy = new MessageOnlyError();
Check(legacy.Message == "Legacy error" && legacy.Code is null && legacy.Domain is null && legacy.ResponseId is null,
    "A message-only implementation must keep the default metadata.");
IAdError structured = new AdError("No fill", 3, "test-domain", "response-123");
Check(structured.Message == "No fill" && structured.Code == 3
    && structured.Domain == "test-domain" && structured.ResponseId == "response-123",
    "The structured error did not preserve its metadata.");
Console.WriteLine("PASS: Structured errors preserve metadata and message-only implementations keep the defaults.");

[MethodImpl(MethodImplOptions.NoInlining)]
static (WeakReference<TestAd> Ad, PaidEventHandler Handler) LoadUnshownAd()
{
    var ad = new TestAd();
    return (new WeakReference<TestAd>(ad), PaidEventHandlers.Create(ad, static (target, value) => target.ReportPaid(value)));
}

static void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

internal sealed class TestAd
{
    public event Action<AdValue>? Paid;

    public void ReportPaid(AdValue value) => Paid?.Invoke(value);
}

internal sealed class MessageOnlyError : IAdError
{
    public string Message => "Legacy error";
}
