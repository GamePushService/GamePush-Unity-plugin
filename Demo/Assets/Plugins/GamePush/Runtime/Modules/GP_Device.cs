using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Device : MonoBehaviour
    {
        public static event UnityAction OnChangeOrientation;
         private void CallChangeOrientation() => OnChangeOrientation?.Invoke();

        [DllImport("__Internal")]
        private static extern string GP_IsMobile();
        public static bool IsMobile()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "true";
#else
            if (GP_ConsoleController.Instance.DeviceConsoleLogs)
                Console.Log("IS MOBILE: ", "TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsMobile;
#endif
        }
        public static bool IsDesktop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "false";
#else
            if (GP_ConsoleController.Instance.DeviceConsoleLogs)
                Console.Log("IS DESKTOP: ", "TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsDesktop;
#endif
        }

         [DllImport("__Internal")]
        private static extern string GP_IsPortrait();
        public static bool IsPortrait()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsPortrait() == "true";
#else
            if (GP_ConsoleController.Instance.DeviceConsoleLogs)
                Console.Log("IS PORTRAIT: ", "?");
            return true;
#endif
        }
    }

}