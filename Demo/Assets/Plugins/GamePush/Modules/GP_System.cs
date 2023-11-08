using System.Runtime.InteropServices;
using UnityEngine;

using GP_Utilities.Console;

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
            return true;
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
            return true;
#endif
        }

    }

}