#if ANDROID
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
#elif IOS
using Google.MobileAds;
using Microsoft.Maui.ApplicationModel;
#endif

namespace Foo.Bar.SampleApp;

internal static class AdInspector
{
    internal static Task<string?> OpenAsync()
    {
        var closed = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

#if ANDROID
        MobileAds.OpenAdInspector(new InspectorClosedListener(closed));
#elif IOS
        if (Platform.GetCurrentUIViewController() is not { } viewController)
        {
            return Task.FromResult<string?>("Ad Inspector needs a visible iOS screen.");
        }

        MobileAds.SharedInstance.PresentAdInspectorFromViewController(
            viewController, error => closed.TrySetResult(error?.LocalizedDescription));
#endif

        return closed.Task;
    }

#if ANDROID
    private sealed class InspectorClosedListener(TaskCompletionSource<string?> completion)
        : Java.Lang.Object, IOnAdInspectorClosedListener
    {
        public void OnAdInspectorClosed(AdInspectorError? error) => completion.TrySetResult(error?.Message);
    }
#endif
}
