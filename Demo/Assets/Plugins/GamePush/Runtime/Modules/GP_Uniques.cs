using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Uniques : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Uniques);
        
        [DllImport("__Internal")]
        private static extern void GP_UniquesRegister(string tag, string value);
        public static void Register(string tag, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesRegister(tag, value);
#else
            ConsoleLog("Register");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_UniquesGet(string tag);
        public static void Get(string tag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesGet(tag);
#else
            ConsoleLog("Get");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_UniquesList();
        public static void List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesList();
#else
            ConsoleLog("List");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_UniquesCheck(string tag, string value);
        public static void Check(string tag, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesCheck(tag, value);
#else
            ConsoleLog("List");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_UniquesDelete(string tag);
        public static void Delete(string tag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_UniquesDelete(tag);
#else
            ConsoleLog("Delete");
#endif
        }
    }
}
