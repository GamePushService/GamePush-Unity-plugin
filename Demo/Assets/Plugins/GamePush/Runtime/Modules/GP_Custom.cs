using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public class GP_Custom : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void GP_CustomCall1(string call);
        public static void CustomCall1(string call)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_CustomCall1(call);
#else
             Console.Log("CUSTOM CALL1: ", "Test");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_CustomCall2(string call);
        public static void CustomCall2(string call)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_CustomCall2(call);
#else
            Console.Log("CUSTOM CALL2: ", "Test");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_CustomCall3(string call);
        public static string CustomCall3(string call)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_CustomCall3(call);
#else
            Console.Log("CUSTOM CALL3: ", "Test");
            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_CustomCall4(string call);
        public static string CustomCall4(string call)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_CustomCall4(call);
#else
            Console.Log("CUSTOM CALL4: ", "Test");
            return null;
#endif
        }


    }

}
