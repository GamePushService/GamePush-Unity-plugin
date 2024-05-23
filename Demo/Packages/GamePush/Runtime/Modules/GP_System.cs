using System.Runtime.InteropServices;
using UnityEngine;

using GamePush.Tools;

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
            GP_Logger.Log("SYSTEM: IS DEV: ", "TRUE");
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
            GP_Logger.Log("SYSTEM: IS ALLOWED ORIGIN: ", "TRUE");
            return true;
#endif
        }

    }

}