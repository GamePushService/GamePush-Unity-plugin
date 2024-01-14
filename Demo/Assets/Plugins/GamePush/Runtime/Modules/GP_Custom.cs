using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public class GP_Custom : MonoBehaviour
    {

        [DllImport("__Internal")]
        private static extern void GP_CustomCall(string call, string args);
        public static void CustomCall(string call, string args = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_CustomCall(call, args);
#else
            Console.Log("CUSTOM Call: ", "Test");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_CustomReturn(string call, string args);
        public static string CustomReturn(string call, string args)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_CustomReturn(call, args);
#else
            Console.Log("CUSTOM Return: ", "Test");
            return null;
#endif
        }




    }

}
