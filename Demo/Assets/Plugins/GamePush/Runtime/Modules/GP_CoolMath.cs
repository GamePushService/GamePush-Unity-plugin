using System.Runtime.InteropServices;

namespace GamePush
{
    public static class GP_CoolMath
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.CoolMath);

        public static void PlayClicked()
        {
            Send("start", withLevel: true, level: 0);
        }

        public static void LevelStart(int level)
        {
            Send("start", withLevel: true, level);
        }

        public static void LevelReplay(int level)
        {
            Send("replay", withLevel: true, level);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_CoolMath_SendEvent(string eventName, int withLevel, int level);
#endif

        private static void Send(string eventName, bool withLevel, int level)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_CoolMath_SendEvent(eventName, withLevel ? 1 : 0, level);
#else
            ConsoleLog(withLevel ? $"{eventName} {level}" : eventName);
#endif
        }
    }
}
