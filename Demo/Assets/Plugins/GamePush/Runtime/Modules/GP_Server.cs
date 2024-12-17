using System;
using System.Runtime.InteropServices;

namespace GamePush
{
    public class GP_Server : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Server);

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_ServerTime();
#endif

        public static DateTime Time()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return DateTime.Parse(GP_ServerTime(), System.Globalization.CultureInfo.InvariantCulture);
#else
            return CoreSDK.GetServerTime();
#endif
        }

    }
}
