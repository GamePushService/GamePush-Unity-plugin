using System.Runtime.InteropServices;
using UnityEngine;
using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Logger : MonoBehaviour
    {
        private static void ColorMessage(string color, string type, string title, string text)
        {
            if(text == null)
                Debug.Log($"<color=#{color}> {type}: </color> {title}");
            else
                Debug.Log($"<color=#{color}> {type}: </color> {title}: {text}");
        }

        private static void GreenMessage(string title, string text) =>
            ColorMessage("4CA57D", "INFO", title, text);
        private static void OrangeMessage(string title, string text) =>
            ColorMessage("D16A31", "WARN", title, text);
        private static void RedMessage(string title, string text) =>
            ColorMessage("CE342A", "ERR", title, text);
        private static void LogMessage(string title, string text)
        {
            string log = text == null ? $" GP: {title}" : $" GP: {title}: {text}";
            Debug.Log(log);
        }


        [DllImport("__Internal")]
        private static extern string GP_LoggerInfo(string title, string text);
        public static void Info(string title = "", string text = null)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            if(GP_Init.isReady) GP_LoggerInfo(title, text);
#else
            GreenMessage(title, text);
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_LoggerWarn(string title, string text);
        public static void Warn(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            if(GP_Init.isReady) GP_LoggerWarn(title, text);
#else
            OrangeMessage(title, text);
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_LoggerError(string title, string text);
        public static void Error(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            if(GP_Init.isReady) GP_LoggerError(title, text);
#else
            RedMessage(title, text);
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_LoggerLog(string title, string text);
        public static void Log(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            if(GP_Init.isReady) GP_LoggerLog(title, text);
#else
            LogMessage(title, text);
#endif
        }

        public static void ModuleLog(string log, ModuleName name)
        {
            if (GP_ConsoleController.Instance.IsModuleLogs(name))
                Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");
        }

        public static void SystemLog(string text)
        {

#if !UNITY_EDITOR && UNITY_WEBGL
            if(GP_Init.isReady) GP_LoggerLog("System", text);
#else
            Debug.Log("<color=#04bc04> GP: </color> " + $"{text}");
#endif
        }

    }
}
