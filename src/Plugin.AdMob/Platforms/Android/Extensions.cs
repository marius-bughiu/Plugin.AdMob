using Android.Gms.Ads;
using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Platforms.Android;

internal static class Extensions
{
    internal static void ApplyGlobalAdConfiguration(this RequestConfiguration.Builder builder)
    {
        builder.SetTestDeviceIds(AdConfig.TestDevices);

        if (AdConfig.TagForChildDirectedTreatment != TagForChildDirectedTreatment.None)
        {
            var tag = AdConfig.TagForChildDirectedTreatment switch
            {
                TagForChildDirectedTreatment.True => RequestConfiguration.TagForChildDirectedTreatmentTrue,
                TagForChildDirectedTreatment.False => RequestConfiguration.TagForChildDirectedTreatmentFalse,
                TagForChildDirectedTreatment.Unspecified => RequestConfiguration.TagForChildDirectedTreatmentUnspecified,
                _ => throw new NotSupportedException($"Unsupported TagForChildDirectedTreatment value: {AdConfig.TagForChildDirectedTreatment}")
            };

            builder.SetTagForChildDirectedTreatment(tag);
        }

        if (AdConfig.TagForUnderAgeOfConsent != TagForUnderAgeOfConsent.None)
        {
            var tag = AdConfig.TagForUnderAgeOfConsent switch
            {
                TagForUnderAgeOfConsent.True => RequestConfiguration.TagForUnderAgeOfConsentTrue,
                TagForUnderAgeOfConsent.False => RequestConfiguration.TagForUnderAgeOfConsentFalse,
                TagForUnderAgeOfConsent.Unspecified => RequestConfiguration.TagForUnderAgeOfConsentUnspecified,
                _ => throw new NotSupportedException($"Unsupported TagForUnderAgeOfConsent value: {AdConfig.TagForUnderAgeOfConsent}")
            };

            builder.SetTagForUnderAgeOfConsent(tag);
        }

        if (AdConfig.MaxAdContentRating != MaxAdContentRating.None)
        {
            var tag = AdConfig.MaxAdContentRating switch
            {
                MaxAdContentRating.G => RequestConfiguration.MaxAdContentRatingG,
                MaxAdContentRating.PG => RequestConfiguration.MaxAdContentRatingPg,
                MaxAdContentRating.T => RequestConfiguration.MaxAdContentRatingT,
                MaxAdContentRating.MA => RequestConfiguration.MaxAdContentRatingMa,
                _ => throw new NotSupportedException($"Unsupported MaxAdContentRating value: {AdConfig.MaxAdContentRating}")
            };

            builder.SetMaxAdContentRating(tag);
        }
    }
}
