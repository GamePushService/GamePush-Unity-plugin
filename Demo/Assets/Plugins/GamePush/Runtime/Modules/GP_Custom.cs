using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public class GP_Custom : MonoBehaviour
    {

        [DllImport("__Internal")]
        private static extern void GP_CustomCall(string name, string args);
        public static void SimpleCall(string name, string args = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_CustomCall(name, args);
#else
            Console.Log("CUSTOM Call: ", "Test");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_CustomReturn(string name, string args);
        public static string GetReturn(string name, string args = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string result = GP_CustomReturn(name, args);
            //Debug.Log("res: " + result.ToString());
            return result;
#else
            Console.Log("CUSTOM Return: ", "Test");
            return null;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_CustomGetValue(string path);
        public static string GetValue(string path)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_CustomGetValue(path);
#else
            Console.Log("CUSTOM value: ", "Test");
            return null;
#endif
        }

    }

}
