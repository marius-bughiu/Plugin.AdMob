using System.Security.Cryptography;
using System.Text;
using Plugin.AdMob.Services;

namespace Plugin.AdMob.DeviceTests;

/// <summary>
/// Observes the plugin's automatic consent flow from app start, so the harness can assert that
/// UMP was actually asked (Android OnStart, iOS app or scene activation) before loading ads.
/// </summary>
internal static class HarnessConsent
{
    private static readonly TaskCompletionSource<(bool Passed, string Detail)> _result =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static DateTime ConsentAt { get; private set; } = DateTime.MaxValue;

    public static void Observe(IAdConsentService consentService)
    {
        consentService.OnConsentInfoUpdated += (_, info) =>
        {
            if (info?.CanRequestAds is true && _result.TrySetResult((true, $"status={info.Status} canRequestAds={info.CanRequestAds}")))
            {
                ConsentAt = DateTime.UtcNow;
            }
        };

        consentService.OnConsentInfoFailedToUpdate += (_, error) => _result.TrySetResult((false, $"consent update failed: {error.Message}"));
        consentService.OnConsentFormError += (_, error) => _result.TrySetResult((false, $"consent form failed: {error.Message}"));
    }

    public static async Task<(bool Passed, string Detail)> WaitAsync(TimeSpan timeout)
    {
        var finished = await Task.WhenAny(_result.Task, Task.Delay(timeout));

        return finished == _result.Task
            ? await _result.Task
            : (false, $"no consent update within {timeout.TotalSeconds}s");
    }

    public static IEnumerable<string> TestDeviceHashedIds()
    {
#if ANDROID
        var androidId = Android.Provider.Settings.Secure.GetString(
            Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

        if (androidId is not null)
        {
            yield return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(androidId)));
        }
#else
        yield break;
#endif
    }
}
