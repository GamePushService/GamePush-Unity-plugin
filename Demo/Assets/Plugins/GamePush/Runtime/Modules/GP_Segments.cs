using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Segments : MonoBehaviour
    {
        public static event UnityAction<string> OnSegmentEnter;
        public static event UnityAction<string> OnSegmentLeave;

        private void CallOnSegmentEnter(string tag) { OnSegmentEnter?.Invoke(tag); }
        private void CallOnSegmentLeave(string tag) { OnSegmentLeave?.Invoke(tag); }

        [DllImport("__Internal")]
        private static extern string GP_Segments_List();
        public static string List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Segments_List();
#else
            GP_Logger.Log("SEGMENTS: ", "LIST");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Segments_Has(string tag);
        public static bool Has(string tag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Segments_Has(tag) == "true";
#else
            GP_Logger.Log("SEGMENTS: ", tag);
            return false;
#endif
        }
    }
}