using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Google.Android.Libraries.Ads.Mobile.Sdk;
using Plugin.AdMob;
using Plugin.AdMob.Platforms.Android;

ConsentGateTests.Run();

bool missingId = args.Contains("--missing-id");
var initial = MobileAdsInitialization.Completed;
var loads = new ConcurrentBag<string>();
var errors = new ConcurrentDictionary<string, Exception>();
int failures = 0;

void Request(string id) => AdMobInitializer.RunWhenInitialized(() => loads.Add(id), error => RecordFailure(id, error));

void RecordFailure(string id, Exception error)
{
    Interlocked.Increment(ref failures);
    errors.TryAdd(id, error);
}

Check(!initial.IsCompleted);

if (missingId)
{
    Android.Content.PM.MetaData.AppId = null;
    Request("manifest");
    var startupException = await AwaitError(initial);
    await WaitFor(() => errors.ContainsKey("manifest"));
    Check(startupException.Message.Contains("application ID is missing"));
    Check(ReferenceEquals(errors["manifest"], startupException));
    Check(MobileAds.Attempts.IsEmpty && loads.IsEmpty);
    Android.Content.PM.MetaData.AppId = "test-app-id";
    Request("manifest-retry");
    var retry = MobileAdsInitialization.Completed;
    Check(!ReferenceEquals(initial, retry));
    var attempt = await NextAttempt();
    attempt.AllowCompletion.TrySetResult(true);
    await retry.WaitAsync(TimeSpan.FromSeconds(5));
    await WaitFor(() => loads.Contains("manifest-retry"));
    Check(loads.Count == 1 && errors.Count == 1 && failures == 1);
    Console.WriteLine("PASS: missing manifest app ID faults the task and request; a new request recovers after correcting the ID.");
    return;
}

MobileAdsInitialization.Start();
var firstAttempt = await NextAttempt();
Request("first");
Request("second");
Check(ReferenceEquals(MobileAdsInitialization.Completed, initial) && !initial.IsCompleted);
firstAttempt.AllowCompletion.TrySetResult(false);

var failure = await AwaitError(initial);
await WaitFor(() => errors.Count == 2);
Check(loads.IsEmpty && ReferenceEquals(errors["first"], failure) && ReferenceEquals(errors["second"], failure));
Console.WriteLine("PASS: concurrent pending requests fail once with the same exception as initialization.");

MobileAdsInitialization.Start();
Check(ReferenceEquals(MobileAdsInitialization.Completed, initial) && MobileAds.Attempts.IsEmpty);
Request("retry");
var retryTask = MobileAdsInitialization.Completed;
var retryAttempt = await NextAttempt();
Check(!ReferenceEquals(initial, retryTask) && !retryTask.IsCompleted);
firstAttempt.CompleteLate();
Check(!retryTask.IsCompleted && loads.IsEmpty);
Request("during retry");
retryAttempt.AllowCompletion.TrySetResult(true);
await retryTask.WaitAsync(TimeSpan.FromSeconds(5));
await WaitFor(() => loads.Count == 2);
Check(loads.Contains("retry") && loads.Contains("during retry") && errors.Count == 2 && failures == 2 && initial.IsFaulted);
Console.WriteLine("PASS: retry publishes a new task, ignores stale completion, and only starts requests queued for the retry.");

Request("already initialized");
Check(loads.Contains("already initialized"));
var requestError = new InvalidOperationException("request failed");
AdMobInitializer.RunWhenInitialized(() => throw requestError, error => RecordFailure("individual request", error));
await WaitFor(() => errors.Count == 3);
Check(ReferenceEquals(errors["individual request"], requestError) && failures == 3 && retryTask.IsCompletedSuccessfully);
Console.WriteLine("PASS: successful initialization is reused and request exceptions retain their own failure callback.");

static async Task<Exception> AwaitError(Task<IInitializationStatus> attempt)
{
    try
    {
        await attempt.WaitAsync(TimeSpan.FromSeconds(5));
    }
    catch (Exception ex) when (ex is not TimeoutException)
    {
        return ex;
    }

    throw new Exception("Expected a startup error.");
}

static async Task<MobileAds.Attempt> NextAttempt()
{
    await WaitFor(() => !MobileAds.Attempts.IsEmpty);
    Check(MobileAds.Attempts.TryDequeue(out var attempt));
    return attempt!;
}

static async Task WaitFor(Func<bool> done)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    while (!done())
        await Task.Delay(10, timeout.Token);
}

static void Check(bool condition, [CallerArgumentExpression(nameof(condition))] string? expression = null)
{
    if (!condition)
        throw new Exception($"Unexpected initializer behavior. Failed assertion: {expression}");
}
