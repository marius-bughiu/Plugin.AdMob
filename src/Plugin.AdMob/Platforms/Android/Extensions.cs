using Google.Android.Libraries.Ads.Mobile.Sdk.Common;
using Plugin.AdMob.Configuration;
using SdkRequestConfiguration = Google.Android.Libraries.Ads.Mobile.Sdk.Common.RequestConfiguration;

namespace Plugin.AdMob.Platforms.Android;

internal static class Extensions
{
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
