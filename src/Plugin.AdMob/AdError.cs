namespace Plugin.AdMob;

/// <summary>
/// An error which occurred while loading an ad.
/// </summary>
public interface IAdError
{
    /// <summary>
    /// The error message.
    /// </summary>
    string Message { get; }
}

internal class AdError(string message) 
    : IAdError
{
    public string Message { get; } = message;
}
