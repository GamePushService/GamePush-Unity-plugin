using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public class GP_Analytics : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Analytics);

        [DllImport("__Internal")]
        private static extern void GP_Analytics_Hit(string url);
        public static void Hit(string url)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_Analytics_Hit(url);
#else

            ConsoleLog("HIT: URL: " + url);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Analytics_Goal(string eventName, string value);
        public static void Goal(string eventName, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Analytics_Goal(eventName, value);
#else

            ConsoleLog("GOAL: EVENT: " + eventName + " VALUE: " + value);
#endif
        }
        public static void Goal(string eventName, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Analytics_Goal(eventName, value.ToString());
#else

            ConsoleLog("GOAL: EVENT: " + eventName + " VALUE: " + value);
#endif
        }
    }
}