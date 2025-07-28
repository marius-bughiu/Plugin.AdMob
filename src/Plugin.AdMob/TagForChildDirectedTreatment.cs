namespace Plugin.AdMob;

/// <summary>
/// For purposes of the Children's Online Privacy Protection Act (COPPA).
/// </summary>
public enum TagForChildDirectedTreatment
{
    /// <summary>
    /// No tag will be specified on ad requests.
    /// </summary>
    None,

    /// <summary>
    /// Indicate that you want your content treated as child-directed for purposes of COPPA.
    /// </summary>
    True,

    /// <summary>
    /// Indicate that you don't want your content treated as child-directed for purposes of COPPA.
    /// </summary>
    False,

    /// <summary>
    /// Indicate that you have not specified how you would like your content treated with respect to COPPA in ad requests.
    /// </summary>
    Unspecified
}
