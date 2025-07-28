namespace Plugin.AdMob;

/// <summary>
/// Ad rating values. Each rating is cumulative. 
/// For example, if you choose T, the ad content will include ads rated G, PG, and T, but block ads rated MA. 
/// </summary>
public enum MaxAdContentRating
{
    /// <summary>
    /// No tag will be specified on ad requests.
    /// </summary>
    None,

    /// <summary>
    /// Indicate that your app can only show G-rated ads.
    /// </summary>
    G,

    /// <summary>
    /// Indicate that your app can only show G- and PG-rated ads.
    /// </summary>
    PG,

    /// <summary>
    /// Indicate that your app can only show G-, PG- and T-rated ads.
    /// </summary>
    T,

    /// <summary>
    /// Indicate that your app can show MA-rated ads.
    /// </summary>
    MA
}
