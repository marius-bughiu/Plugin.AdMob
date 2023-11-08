using Android.Content.Res;
using Android.Gms.Ads;
using Android.Util;
using Microsoft.Maui.Handlers;
using Plugin.AdMob.Configuration;
using Plugin.AdMob.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.MediaStore;

namespace Plugin.AdMob.Handlers
{
    public partial class BannerAdHandler : ViewHandler<BannerAd, AdView>
    {
        public static IPropertyMapper<BannerAd, BannerAdHandler> PropertyMapper = new PropertyMapper<BannerAd, BannerAdHandler>(ViewMapper)
        {

        };

        public BannerAdHandler() : base(PropertyMapper)
        {
            
        }

        protected override void ConnectHandler(AdView platformView)
        {
            base.ConnectHandler(platformView);

            // Perform any control setup here
        }

        protected override void DisconnectHandler(AdView platformView)
        {
            // Perform any native view cleanup here
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        protected override AdView CreatePlatformView()
        {
            var adView = new AdView(Context)
            {
                AdSize = GetAdSize(),
                AdUnitId = VirtualView.AdUnitId
            };

            if (string.IsNullOrEmpty(adView.AdUnitId) && !string.IsNullOrEmpty(AdConfig.DefaultBannerAdUnitId))
            {
                adView.AdUnitId = AdConfig.DefaultBannerAdUnitId;
            }

            if (AdConfig.UseTestAdUnitIds)
            {
                adView.AdUnitId = AdMobTestAdUnits.Banner;
            }

            var requestBuilder = new AdRequest.Builder();
            foreach (var testDeviceId in AdConfig.TestDevices)
            {
                // TODO: is this a broken library binding or was the method removed?
                // requestBuilder.AddTestDevice(testDeviceId);
            }

            adView.LoadAd(requestBuilder.Build());

            VirtualView.HeightRequest = GetSmartBannerHeightDp();

            return adView;
        }

        private int GetSmartBannerHeightDp()
        {
            var displayMetrics = new DisplayMetrics();
            Context.Display.GetMetrics(displayMetrics);

            var screenHeightDp = displayMetrics.HeightPixels / displayMetrics.Density;
            return BannerSizeHelper.GetSmartBannerHeight(screenHeightDp);
        }
        private Android.Gms.Ads.AdSize GetAdSize()
        {
            // TODO: Use GetCurrentOrientationAnchoredAdaptiveBannerAdSize instead of SmartBanner

            switch (VirtualView.AdSize)
            {
                case AdSize.Banner: return Android.Gms.Ads.AdSize.Banner;
                case AdSize.LargeBanner: return Android.Gms.Ads.AdSize.LargeBanner;
                case AdSize.MediumRectangle: return Android.Gms.Ads.AdSize.MediumRectangle;
                case AdSize.FullBanner: return Android.Gms.Ads.AdSize.FullBanner;
                case AdSize.Leaderboard: return Android.Gms.Ads.AdSize.Leaderboard;
                case AdSize.Custom: return new Android.Gms.Ads.AdSize(VirtualView.CustomAdWidth, VirtualView.CustomAdHeight);
                default: return Android.Gms.Ads.AdSize.SmartBanner;                    
            }
        }
    }
}
