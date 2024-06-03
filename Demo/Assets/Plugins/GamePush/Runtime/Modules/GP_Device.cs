using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;

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
            bool isMobile = false;
            GP_Logger.Log("IS MOBILE: ", isMobile.ToString());
            return isMobile;
#endif
        }
        public static bool IsDesktop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "false";
#else
            bool isDesktop = true;
            GP_Logger.Log("IS DESKTOP: ", isDesktop.ToString());
            return isDesktop;
#endif
        }

         [DllImport("__Internal")]
        private static extern string GP_IsPortrait();
        public static bool IsPortrait()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsPortrait() == "true";
#else
            bool isPortrait = false;
            GP_Logger.Log("IS PORTRAIT: ", isPortrait.ToString());
            return isPortrait;
#endif
        }
    }

}