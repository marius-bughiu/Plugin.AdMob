using Google.MobileAds;
using Plugin.AdMob.Configuration;

namespace Plugin.AdMob.Platforms.iOS;

internal static class Extensions
{
    internal static void ApplyGlobalAdConfiguration(this RequestConfiguration requestConfiguration)
    {
        requestConfiguration.TestDeviceIdentifiers = [.. AdConfig.TestDevices];

        if (AdConfig.TagForChildDirectedTreatment != TagForChildDirectedTreatment.None)
        {
            var tag = AdConfig.TagForChildDirectedTreatment switch
            {
                TagForChildDirectedTreatment.True => 1,
                TagForChildDirectedTreatment.False => 0,
                TagForChildDirectedTreatment.Unspecified => -1,
                _ => throw new NotSupportedException($"Unsupported TagForChildDirectedTreatment value: {AdConfig.TagForChildDirectedTreatment}")
            };

            requestConfiguration.TagForChildDirectedTreatment = tag;
        }

        if (AdConfig.TagForUnderAgeOfConsent != TagForUnderAgeOfConsent.None)
        {
            var tag = AdConfig.TagForUnderAgeOfConsent switch
            {
                TagForUnderAgeOfConsent.True => 1,
                TagForUnderAgeOfConsent.False => 0,
                TagForUnderAgeOfConsent.Unspecified => -1,
                _ => throw new NotSupportedException($"Unsupported TagForUnderAgeOfConsent value: {AdConfig.TagForUnderAgeOfConsent}")
            };

            requestConfiguration.TagForUnderAgeOfConsent = tag;
        }

        if (AdConfig.MaxAdContentRating != MaxAdContentRating.None)
        {
            var tag = AdConfig.MaxAdContentRating switch
            {
                MaxAdContentRating.G => MaxAdContentRatingConstants.General,
                MaxAdContentRating.PG => MaxAdContentRatingConstants.ParentalGuidance,
                MaxAdContentRating.T => MaxAdContentRatingConstants.Teen,
                MaxAdContentRating.MA => MaxAdContentRatingConstants.MatureAudience,
                _ => throw new NotSupportedException($"Unsupported MaxAdContentRating value: {AdConfig.MaxAdContentRating}")
            };

            requestConfiguration.MaxAdContentRating = tag;
        }
    }
}
