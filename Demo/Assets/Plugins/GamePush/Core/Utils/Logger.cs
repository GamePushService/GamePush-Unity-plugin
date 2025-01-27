using System;
using UnityEngine;

namespace GamePush.Core
{
    public class Logger
    {
        public static event Action<string> OnLog;

        private static void DebugLog(string log)
        {
            Debug.Log(log);
            OnLog?.Invoke(log);
        }

        private static void ColorLog(string color, string type, string title, string text) =>
            DebugLog($"<color=#{color}> {type}: </color> {title}: {text}");

        private static void ColorLogWarning(string color, string type, string title, string text)
        {
            string log = text == null || text == "" ? title : $"{title}: {text}";
            Debug.LogWarning($"<color=#{color}> {type}: </color> {log}");
            OnLog?.Invoke(log);
        }
            

        private static void ColorLogError(string color, string type, string title, string text)
        {
            string log = text == null || text == "" ? title : $"{title}: {text}";
            Debug.LogError($"<color=#{color}> {type}: </color> {log}");
            OnLog?.Invoke(log);
        }
            


        private static void GreenMessage(string title, string text) =>
            ColorLog("4CA57D", "INFO", title, text);
        private static void OrangeMessage(string title, string text) =>
            ColorLogWarning("D16A31", "WARN", title, text);
        private static void RedMessage(string title, string text) =>
            ColorLogError("CE342A", "ERR", title, text);
        private static void LogMessage(string title, string text)
        {
            string log;
            if (text != null)
                log = $"{title}: {text}";
            else
                log = title;

            DebugLog(log);
        }
            


        public static void Info(string title = "", string text = null) => GreenMessage(title, text);
        public static void Warn(string title = "", string text = null) => OrangeMessage(title, text);

        public static void Error(string title = "", string text = null) => RedMessage(title, text);

        public static void Log(string title = "", object text = null) =>
            LogMessage(title, text?.ToString());
        
        public static void Log(object value) =>
            LogMessage(value.ToString(), null);

        public static void ModuleLog(string log, ModuleName name) =>
                DebugLog("<color=#04bc04> GP: </color> " + $"{name}: {log}");
        

        public static void SystemLog(string text) =>
            DebugLog("<color=#04bc04> GP: </color> " + $"{text}");
    }

}
