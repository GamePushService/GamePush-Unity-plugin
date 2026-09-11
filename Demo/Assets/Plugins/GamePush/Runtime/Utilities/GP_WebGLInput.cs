namespace GamePush.Utilities
{
    public static class GP_WebGLInput
    {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
        static int _overlayCount;
#endif

        public static void Release()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            _overlayCount++;
            if (_overlayCount == 1)
                SetCaptureAllKeyboardInput(false);
#endif
        }

        public static void Restore()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            if (_overlayCount <= 0)
                return;
            _overlayCount--;
            if (_overlayCount == 0)
                SetCaptureAllKeyboardInput(true);
#endif
        }

#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
        static void SetCaptureAllKeyboardInput(bool value)
        {
            var webGLInputType = System.Type.GetType("UnityEngine.WebGLInput, UnityEngine.WebGLModule");
            var captureProperty = webGLInputType?.GetProperty("captureAllKeyboardInput");
            captureProperty?.SetValue(null, value);
        }
#endif
    }
}
