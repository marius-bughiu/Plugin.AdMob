namespace Plugin.AdMob.Helpers;

internal class BannerSizeHelper
{
    internal static int GetSmartBannerHeight(float screenHeightDp)
    {
        if (screenHeightDp <= 400)
        {
            return 32;
        }

        if (screenHeightDp <= 720)
        {
            return 50;
        }

        return 90;
    }
}
