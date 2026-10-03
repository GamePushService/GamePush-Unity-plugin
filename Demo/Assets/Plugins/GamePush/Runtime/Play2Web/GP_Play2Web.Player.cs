#if !UNITY_EDITOR
namespace GamePush
{
    public static class GP_Play2Web
    {
        public static bool Enabled => false;
        public static bool IsReady => false;

        public static bool Call(string method, params object[] args) => false;

        public static bool TryGet(string key, out string value)
        {
            value = null;
            return false;
        }

        public static bool TryGetBool(string key, out bool value)
        {
            value = false;
            return false;
        }

        public static bool TryGetInt(string key, out int value)
        {
            value = 0;
            return false;
        }

        public static bool TryGetFloat(string key, out float value)
        {
            value = 0;
            return false;
        }
    }
}
#endif
