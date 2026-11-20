using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Services;

internal sealed class ConsentGate
{
    private readonly IAdConsentService _adConsentService;

    public ConsentGate(IAdConsentService adConsentService)
    {
        _adConsentService = adConsentService;
    }

    public bool CanRequestAds() => AdConfig.DisableConsentCheck || _adConsentService.CanRequestAds();

    public void WhenAllowed(Action action) => CreateRequest(action).Start();

    public PendingRequest CreateRequest(Action action) => new(this, action);

    internal sealed class PendingRequest(ConsentGate gate, Action action) : IDisposable
    {
        private Action? _action = action;

        public void Start()
        {
#if ANDROID || IOS
            if (!gate.CanRequestAds())
            {
                WaitForConsent();
                return;
            }
#endif

            // Desktop ad loads report that the platform is unsupported without waiting for consent.
            Interlocked.Exchange(ref _action, null)?.Invoke();
        }

        private void WaitForConsent()
        {
            gate._adConsentService.OnConsentInfoUpdated += OnConsentInfoUpdated;

            // The request may have been cancelled while the handler was being attached.
            if (Volatile.Read(ref _action) is null)
            {
                Unsubscribe();
                return;
            }

            try
            {
                // Catch consent updates that happened before the subscription was in place.
                TryRun();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _action, null);
            Unsubscribe();
        }

        private void OnConsentInfoUpdated(object? sender, IConsentInformation? information) => TryRun();

        private void TryRun()
        {
            if (gate.CanRequestAds() && Interlocked.Exchange(ref _action, null) is { } callback)
            {
                Unsubscribe();
                callback();
            }
        }

        private void Unsubscribe() => gate._adConsentService.OnConsentInfoUpdated -= OnConsentInfoUpdated;
    }
}
