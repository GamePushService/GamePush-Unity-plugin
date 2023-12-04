using System.Runtime.InteropServices;
using UnityEngine;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Analytics : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void GP_Analytics_Hit(string url);
        public static void Hit(string url)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_Analytics_Hit(url);
#else
            if (GP_ConsoleController.Instance.AnalyticsConsoleLogs)
                Console.Log("ANALYTICS: HIT: ", "URL: " + url);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Analytics_Goal(string eventName, string value);
        public static void Goal(string eventName, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Analytics_Goal(eventName, value);
#else
            if (GP_ConsoleController.Instance.AnalyticsConsoleLogs)
                Console.Log("ANALYTICS: GOAL: ", "EVENT: " + eventName + " VALUE: " + value);
#endif
        }
        public static void Goal(string eventName, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Analytics_Goal(eventName, value.ToString());
#else
            if (GP_ConsoleController.Instance.AnalyticsConsoleLogs)
                Console.Log("ANALYTICS: GOAL: ", "EVENT: " + eventName + " VALUE: " + value);
#endif
        }
    }
}