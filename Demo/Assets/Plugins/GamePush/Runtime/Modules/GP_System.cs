using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System;
using UnityEngine;

using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_System : MonoBehaviour
    {

        [DllImport("__Internal")]
        private static extern string GP_IsDev();
        public static bool IsDev()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsDev() == "true";
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("SYSTEM: IS DEV: ", "TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsDev;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_IsAllowedOrigin();
        public static bool IsAllowedOrigin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsAllowedOrigin() == "true";
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("SYSTEM: IS ALLOWED ORIGIN: ", "TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsAllowedOrigin;
#endif
        }

       
    }

}