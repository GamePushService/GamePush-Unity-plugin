using System.Runtime.InteropServices;
using UnityEngine;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Device : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern string GP_IsMobile();
        public static bool IsMobile()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "true";
#else
            if (GP_ConsoleController.Instance.DeviceConsoleLogs)
                Console.Log("IS MOBILE: ", "TRUE");
            return true;
#endif
        }
        public static bool IsDesktop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "false";
#else
            if (GP_ConsoleController.Instance.DeviceConsoleLogs)
                Console.Log("IS DESKTOP: ", "TRUE");
            return true;
#endif
        }
    }

}