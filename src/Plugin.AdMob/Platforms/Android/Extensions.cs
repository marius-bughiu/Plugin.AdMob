using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Google.Android.Libraries.Ads.Mobile.Sdk.Initialization;
using Plugin.AdMob.Configuration;
using SdkIInitializationStatus = Google.Android.Libraries.Ads.Mobile.Sdk.Initialization.IInitializationStatus;
using SdkRequestConfiguration = Google.Android.Libraries.Ads.Mobile.Sdk.Common.RequestConfiguration;

namespace Plugin.AdMob.Platforms.Android;

internal static class Extensions
{
    internal static AdError ToAdError(this LoadAdError error)
        => new(error.Message, error.Code?.Value, null, error.ResponseInfo?.ResponseId);

    internal static AdError ToAdError(this FullScreenContentError error, string? responseId)
        => new(error.Message, error.Code?.Value, null, responseId);

    internal static AdRevenueEventArgs ToRevenueEventArgs(this AdValue value, string adUnitId)
    {
        var precision = value.PrecisionType;
        var revenuePrecision = precision switch
        {
            _ when PrecisionType.Estimated?.Equals(precision) == true => AdRevenuePrecision.Estimated,
            _ when PrecisionType.PublisherProvided?.Equals(precision) == true => AdRevenuePrecision.PublisherProvided,
            _ when PrecisionType.Precise?.Equals(precision) == true => AdRevenuePrecision.Precise,
            _ => AdRevenuePrecision.Unknown,
        };

        return new AdRevenueEventArgs(adUnitId, value.ValueMicros / 1_000_000m, value.CurrencyCode, revenuePrecision);
    }

    internal static InitializationStatus ToInitializationStatus(this SdkIInitializationStatus status) => new(
    [
        .. status.AdapterStatusMap.Select(adapter => new AdapterStatus(
            adapter.Key,
            AdapterStatusInitializationState.Complete!.Equals(adapter.Value.InitializationState) ? AdapterState.Ready : AdapterState.NotReady,
            adapter.Value.Description,
            TimeSpan.FromMilliseconds(adapter.Value.Latency)))
    ]);

    internal static void ApplyGlobalAdConfiguration(this SdkRequestConfiguration.Builder builder)
    {
        builder.SetTestDeviceIds(AdConfig.TestDevices);

        if (AdConfig.TagForChildDirectedTreatment != TagForChildDirectedTreatment.None)
        {
            var tag = AdConfig.TagForChildDirectedTreatment switch
            {
                TagForChildDirectedTreatment.True => SdkRequestConfiguration.TagForChildDirectedTreatment.TagForChildDirectedTreatmentTrue,
                TagForChildDirectedTreatment.False => SdkRequestConfiguration.TagForChildDirectedTreatment.TagForChildDirectedTreatmentFalse,
                TagForChildDirectedTreatment.Unspecified => SdkRequestConfiguration.TagForChildDirectedTreatment.TagForChildDirectedTreatmentUnspecified,
                _ => throw new NotSupportedException($"Unsupported TagForChildDirectedTreatment value: {AdConfig.TagForChildDirectedTreatment}")
            };

            builder.SetTagForChildDirectedTreatment(tag!);
        }

        if (AdConfig.TagForUnderAgeOfConsent != TagForUnderAgeOfConsent.None)
        {
            var tag = AdConfig.TagForUnderAgeOfConsent switch
            {
                TagForUnderAgeOfConsent.True => SdkRequestConfiguration.TagForUnderAgeOfConsent.TagForUnderAgeOfConsentTrue,
                TagForUnderAgeOfConsent.False => SdkRequestConfiguration.TagForUnderAgeOfConsent.TagForUnderAgeOfConsentFalse,
                TagForUnderAgeOfConsent.Unspecified => SdkRequestConfiguration.TagForUnderAgeOfConsent.TagForUnderAgeOfConsentUnspecified,
                _ => throw new NotSupportedException($"Unsupported TagForUnderAgeOfConsent value: {AdConfig.TagForUnderAgeOfConsent}")
            };

            builder.SetTagForUnderAgeOfConsent(tag!);
        }

        if (AdConfig.MaxAdContentRating != MaxAdContentRating.None)
        {
            var tag = AdConfig.MaxAdContentRating switch
            {
                MaxAdContentRating.G => SdkRequestConfiguration.MaxAdContentRating.MaxAdContentRatingG,
                MaxAdContentRating.PG => SdkRequestConfiguration.MaxAdContentRating.MaxAdContentRatingPg,
                MaxAdContentRating.T => SdkRequestConfiguration.MaxAdContentRating.MaxAdContentRatingT,
                MaxAdContentRating.MA => SdkRequestConfiguration.MaxAdContentRating.MaxAdContentRatingMa,
                _ => throw new NotSupportedException($"Unsupported MaxAdContentRating value: {AdConfig.MaxAdContentRating}")
            };

            builder.SetMaxAdContentRating(tag!);
        }
    }
}
