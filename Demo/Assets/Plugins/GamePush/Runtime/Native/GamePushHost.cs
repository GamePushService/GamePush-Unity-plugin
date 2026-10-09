namespace GamePush.Native
{
    public interface IGamePushHost
    {
        bool UseNativeCore { get; }
        bool UseJsSdk { get; }
    }

    sealed class EditorPlay2WebHost : IGamePushHost
    {
        public bool UseNativeCore => false;
        public bool UseJsSdk => true;
    }

    sealed class WebGlJsHost : IGamePushHost
    {
        public bool UseNativeCore => false;
        public bool UseJsSdk => true;
    }

    sealed class NativeCoreHost : IGamePushHost
    {
        public bool UseNativeCore => true;
        public bool UseJsSdk => false;
    }

    public static class GamePushHost
    {
        public static IGamePushHost Current
        {
            get
            {
#if UNITY_EDITOR
                return new EditorPlay2WebHost();
#elif UNITY_WEBGL && !GP_NATIVE_WEBGL
                return new WebGlJsHost();
#else
                return new NativeCoreHost();
#endif
            }
        }

        public static bool UseNativeCore => Current.UseNativeCore;

        public static bool UseJsAds
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
    }
}
