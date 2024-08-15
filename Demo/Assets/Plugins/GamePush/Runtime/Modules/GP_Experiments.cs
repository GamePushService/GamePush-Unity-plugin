using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;
using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Experiments : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Experiments);

        [DllImport("__Internal")]
        private static extern string GP_Experiments_Map();
        public static string Map()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string map = GP_Experiments_Map();
            return map;
#else

            ConsoleLog("MAP");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Experiments_Has(string tag, string cohort);
        public static bool Has(string tag, string cohort)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Experiments_Has(tag, cohort) == "true";
#else

            ConsoleLog(tag + " | " + cohort);
            return false;
#endif
        }
    }
}
