namespace Plugin.AdMob;

public interface IConsentError
{
    string Message { get; }
}

internal class ConsentError(string message) 
    : IConsentError
{
    public string Message { get; } = message;
}
