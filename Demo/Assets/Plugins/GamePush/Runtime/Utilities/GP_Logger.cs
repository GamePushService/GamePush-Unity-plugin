using System;
using System.Runtime.InteropServices;
using UnityEngine;
using GamePush.ConsoleController;
using GamePush.Core;

namespace GamePush
{
    public class GP_Logger : MonoBehaviour
    {
        public static event Action<string> OnLog;
        private void OnEnable() => Core.Logger.OnLog += InvokeOnLog;
        private void OnDisable() => Core.Logger.OnLog -= InvokeOnLog;

        private void InvokeOnLog(string log) => OnLog?.Invoke(log);

        private static void DebugLog(string log)
        {
            Debug.Log(log);
            OnLog?.Invoke(log);
        }

        private static void ColorMessage(string color, string type, string title, string text) =>
            DebugLog($"<color=#{color}> {type}: </color> {title}: {text}");
           

        private static void GreenMessage(string title, string text) =>
            ColorMessage("4CA57D", "INFO", title, text);
        private static void OrangeMessage(string title, string text) =>
            ColorMessage("D16A31", "WARN", title, text);
        private static void RedMessage(string title, string text) =>
            ColorMessage("CE342A", "ERR", title, text);
        private static void LogMessage(string title, string text) =>
            DebugLog($" GP: {title}: {text}");


        [DllImport("libARWrapper.so")]
        private static extern string GP_LoggerInfo(string title, string text);
        public static void Info(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_LoggerInfo(title, text);
#else
            GreenMessage(title, text);
#endif
        }

        [DllImport("libARWrapper.so")]
        private static extern string GP_LoggerWarn(string title, string text);
        public static void Warn(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_LoggerWarn(title, text);
#else
            OrangeMessage(title, text);
#endif
        }

        [DllImport("libARWrapper.so")]
        private static extern string GP_LoggerError(string title, string text);
        public static void Error(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_LoggerError(title, text);
#else
            RedMessage(title, text);
#endif
        }

        [DllImport("libARWrapper.so")]
        private static extern string GP_LoggerLog(string title, string text);
        public static void Log(string title = "", string text = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_LoggerLog(title, text);
#else
            LogMessage(title, text);
#endif
        }

        public static void ModuleLog(string log, ModuleName name)
        {
            #if UNITY_EDITOR
            if (GP_ConsoleController.Instance.IsModuleLogs(name))
                Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");

            #else
            Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");
            #endif
        }

        public static void SystemLog(string text) =>
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_LoggerLog("System", text);
#else
            Debug.Log("<color=#04bc04> GP: </color> " + $"{text}");
#endif

    }
}
