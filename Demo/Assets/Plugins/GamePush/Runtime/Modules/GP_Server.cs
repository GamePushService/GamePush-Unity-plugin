using System;
using System.Runtime.InteropServices;
using UnityEngine;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Server : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern string GP_ServerTime();

        public static DateTime Time()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return DateTime.Parse(GP_ServerTime(), System.Globalization.CultureInfo.InvariantCulture);
#else
            GP_Logger.Log("SERVER: ", "TIME: " + DateTime.Now);
            return DateTime.Now;
#endif
        }

    }
}
