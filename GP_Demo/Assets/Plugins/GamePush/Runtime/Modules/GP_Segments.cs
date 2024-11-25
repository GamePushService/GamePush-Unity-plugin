﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Segments : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Segments);

        public static event UnityAction<string> OnSegmentEnter;
        public static event UnityAction<string> OnSegmentLeave;

        private void CallOnSegmentEnter(string tag) { OnSegmentEnter?.Invoke(tag); }
        private void CallOnSegmentLeave(string tag) { OnSegmentLeave?.Invoke(tag); }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_Segments_List();
        [DllImport("__Internal")]
        private static extern string GP_Segments_Has(string tag);
#endif

        public static string List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Segments_List();
#else

            ConsoleLog("LIST");

            return null;
#endif
        }

        public static bool Has(string tag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Segments_Has(tag) == "true";
#else

            ConsoleLog("HAS: " + tag);
            return false;
#endif
        }
    }
}