using Plugin.AdMob;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Services;

internal static class ConsentGateTests
{
    internal static void Run()
    {
        var consent = new FakeConsent();
        var gate = new ConsentGate(consent);
        int starts = 0;
        void CountStart() => Interlocked.Increment(ref starts);

        consent.Allowed = true;
        gate.WhenAllowed(CountStart);
        Check("pre-existing consent", 1, 0);

        consent.Allowed = false;
        consent.BeforeSubscribe = () =>
        {
            consent.Allowed = true;
            consent.CaptureNotification()();
        };
        gate.WhenAllowed(CountStart);
        Check("grant between check and subscription", 1, 0);

        consent.Allowed = false;
        gate.WhenAllowed(CountStart);
        consent.CaptureNotification()();
        Check("denied notification keeps waiting", 0, 1);
        consent.Allowed = true;
        var first = consent.CaptureNotification();
        var second = consent.CaptureNotification();
        Parallel.Invoke(first, second);
        Check("overlapping notifications start once", 1, 0);

        consent.Allowed = false;
        consent.BeforeRead = () =>
        {
            consent.BeforeRead = () =>
            {
                consent.Allowed = true;
                consent.CaptureNotification()();
            };
        };
        gate.WhenAllowed(CountStart);
        Check("notification during the post-subscription check", 1, 0);

        consent.Allowed = false;
        using (var pending = gate.CreateRequest(CountStart))
        {
            consent.BeforeSubscribe = pending.Dispose;
            pending.Start();
        }
        Check("cancellation while attaching", 0, 0);

        using (var pending = gate.CreateRequest(CountStart))
        {
            pending.Start();
            var captured = consent.CaptureNotification();
            pending.Dispose();
            consent.Allowed = true;
            captured();
        }
        Check("cancelled subscription ignores a captured notification", 0, 0);

        consent.Allowed = false;
        AdConfig.DisableConsentCheck = true;
        gate.WhenAllowed(CountStart);
        Check("explicit consent-check override", 1, 0);
        AdConfig.DisableConsentCheck = false;

        void Check(string scenario, int expectedStarts, int expectedSubscribers)
        {
            if (starts != expectedStarts || consent.Subscribers != expectedSubscribers)
            {
                throw new Exception($"{scenario}: expected {expectedStarts} starts and {expectedSubscribers} subscriptions, got {starts} and {consent.Subscribers}.");
            }

            Console.WriteLine($"PASS: {scenario}");
            starts = 0;
        }
    }
}

internal sealed class FakeConsent : IAdConsentService
{
    private EventHandler<IConsentInformation?>? _updated;
    public bool Allowed { get; set; }
    public Action? BeforeSubscribe { get; set; }
    public Action? BeforeRead { get; set; }
    public int Subscribers => _updated?.GetInvocationList().Length ?? 0;

    public event EventHandler<IConsentInformation?>? OnConsentInfoUpdated
    {
        add
        {
            var action = BeforeSubscribe;
            BeforeSubscribe = null;
            action?.Invoke();
            _updated += value;
        }
        remove => _updated -= value;
    }

    public event EventHandler<IConsentError>? OnConsentInfoFailedToUpdate;
    public event EventHandler? OnConsentFormDismissed;
    public event EventHandler<IConsentError>? OnConsentFormError;

    public Action CaptureNotification()
    {
        var subscribers = _updated;
        return () => subscribers?.Invoke(this, null);
    }

    public bool CanRequestAds()
    {
        var action = BeforeRead;
        BeforeRead = null;
        action?.Invoke();
        return Allowed;
    }
}
