namespace Plugin.AdMob.Services;

public interface IInterstitialAdService
{
    void PrepareAd(string adUnitId) => throw new NotImplementedException();

    void PrepareAd() => throw new NotImplementedException();

    void ShowAd() => throw new NotImplementedException();
}

internal partial class InterstitialAdService : IInterstitialAdService
{

}
