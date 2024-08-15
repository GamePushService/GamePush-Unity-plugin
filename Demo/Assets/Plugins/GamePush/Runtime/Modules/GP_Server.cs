using System;
using System.Runtime.InteropServices;

namespace GamePush
{
    public class GP_Server : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Server);

        [DllImport("__Internal")]
        private static extern string GP_ServerTime();

        public static DateTime Time()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return DateTime.Parse(GP_ServerTime(), System.Globalization.CultureInfo.InvariantCulture);
#else

            ConsoleLog("TIME: " + DateTime.Now);
            return DateTime.Now;
#endif
        }

    }
}
