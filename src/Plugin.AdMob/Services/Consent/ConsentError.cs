namespace Plugin.AdMob;

/// <summary>
/// An error which occurred while loading the consent form.
/// </summary>
public interface IConsentError
{
    /// <summary>
    /// The error message.
    /// </summary>
    string Message { get; }
}

internal class ConsentError(string message) 
    : IConsentError
{
    public string Message { get; } = message;
}
