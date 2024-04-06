namespace Plugin.AdMob;

public interface IAdError
{
    string Message { get; }
}

internal partial class AdError : IAdError
{
    public string Message { get; }

    public AdError(string message)
    {
        Message = message;
    }
}
