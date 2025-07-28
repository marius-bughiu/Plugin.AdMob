namespace Plugin.AdMob;

/// <summary>
/// Treatment for users in the European Economic Area (EEA) under the age of consent.
/// </summary>
public enum TagForUnderAgeOfConsent
{
    /// <summary>
    /// No tag will be specified on ad requests.
    /// </summary>
    None,

    /// <summary>
    /// Indicate that you want the ad request to receive treatment for users in the European Economic Area (EEA) under the age of consent.
    /// </summary>
    True,

    /// <summary>
    /// Indicate that you want the ad request to not receive treatment for users in the European Economic Area (EEA) under the age of consent.
    /// </summary>
    False,

    /// <summary>
    /// Indicate that you have not specified whether the ad request should receive treatment for users in the European Economic Area (EEA) under the age of consent.
    /// </summary>
    Unspecified
}
