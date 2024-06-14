using System;
using System.Runtime.InteropServices;
using UnityEngine;

using GamePush.ConsoleController;

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
            if (GP_ConsoleController.Instance.ServerConsoleLogs)
                Console.Log("SERVER: ", "TIME: " + DateTime.Now);
            return DateTime.Now;
#endif
        }

    }
}
