using Foundation;
using Google.MobileAds;
using ObjCRuntime;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Google.MobileAds
{
    [Protocol(Name = "GADNativeAdLoaderDelegate", WrapperType = typeof(NativeAdLoaderDelegateWrapper))]
    [ProtocolMember(IsRequired = true, IsProperty = false, IsStatic = false, Name = "DidReceiveNativeAd", Selector = "adLoader:didReceiveNativeAd:", ParameterType = new Type[]
    {
        typeof(AdLoader),
        typeof(Google.MobileAds.NativeAd)
    }, ParameterByRef = new bool[] { false, false })]
    public interface INativeAdLoaderDelegate : INativeObject, IDisposable, IAdLoaderDelegate
    {
        [RequiredMember]
        [Export("adLoader:didReceiveNativeAd:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        void DidReceiveNativeAd(AdLoader adLoader, Google.MobileAds.NativeAd nativeAd)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }

        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        internal static void _DidReceiveNativeAd(INativeAdLoaderDelegate This, AdLoader adLoader, NativeAd nativeAd)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }

        [DynamicDependency("DidReceiveNativeAd(Google.MobileAds.AdLoader,Google.MobileAds.NativeAd)")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        static INativeAdLoaderDelegate()
        {
            GC.KeepAlive(null);
        }
    }

    internal sealed class NativeAdLoaderDelegateWrapper : BaseWrapper, INativeAdLoaderDelegate, INativeObject, IDisposable, IAdLoaderDelegate
    {
        [Preserve(Conditional = true)]
        public NativeAdLoaderDelegateWrapper(NativeHandle handle, bool owns)
            : base(handle, owns)
        {
        }

        [Export("adLoader:didReceiveNativeAd:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public void DidReceiveNativeAd(AdLoader adLoader, Google.MobileAds.NativeAd nativeAd)
        {
            NativeHandle nonNullHandle = adLoader.GetNonNullHandle("adLoader");
            NativeHandle nonNullHandle2 = nativeAd.GetNonNullHandle("nativeAd");
            void_objc_msgSend_NativeHandle_NativeHandle((nint)base.Handle, Selector.GetHandle("adLoader:didReceiveNativeAd:"), nonNullHandle, nonNullHandle2);
        }

        [Export("adLoader:didFailToReceiveAdWithError:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public void DidFailToReceiveAd(AdLoader adLoader, NSError error)
        {
            NativeHandle nonNullHandle = adLoader.GetNonNullHandle("adLoader");
            NativeHandle nonNullHandle2 = error.GetNonNullHandle("error");
            void_objc_msgSend_NativeHandle_NativeHandle((nint)base.Handle, Selector.GetHandle("adLoader:didFailToReceiveAdWithError:"), nonNullHandle, nonNullHandle2);
        }

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_NativeHandle_NativeHandle(nint receiver, nint selector, NativeHandle arg1, NativeHandle arg2);
    }

    [Protocol]
    [Register("ApiDefinition__Google_MobileAds_NativeAdLoaderDelegate", false)]
    [Model]
    public abstract class NativeAdLoaderDelegate : NSObject, INativeAdLoaderDelegate, INativeObject, IDisposable, IAdLoaderDelegate
    {
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Export("init")]
        protected NativeAdLoaderDelegate()
            : base(NSObjectFlag.Empty)
        {
            base.IsDirectBinding = false;
            InitializeHandle(Messaging.IntPtr_objc_msgSendSuper((nint)base.SuperHandle, Selector.GetHandle("init")), "init");
        }

        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected NativeAdLoaderDelegate(NSObjectFlag t)
            : base(t)
        {
            base.IsDirectBinding = false;
        }

        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal NativeAdLoaderDelegate(NativeHandle handle)
            : base(handle)
        {
            base.IsDirectBinding = false;
        }

        [Export("adLoader:didFailToReceiveAdWithError:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void DidFailToReceiveAd(AdLoader adLoader, NSError error)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }

        [Export("adLoaderDidFinishLoading:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void DidFinishLoading(AdLoader adLoader)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }

        [Export("adLoader:didReceiveNativeAd:")]
        [BindingImpl(BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
        public virtual void DidReceiveNativeAd(AdLoader adLoader, NativeAd nativeAd)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }
    }
}
