using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class Logger : MonoBehaviour
    {
        private static void ColorLog(string color, string type, string title, string text) =>
            Debug.Log($"<color=#{color}> {type}: </color> {title}: {text}");

        private static void ColorLogWarning(string color, string type, string title, string text) =>
            Debug.LogWarning($"<color=#{color}> {type}: </color> {title}: {text}");

        private static void ColorLogError(string color, string type, string title, string text) =>
            Debug.LogError($"<color=#{color}> {type}: </color> {title}: {text}");


        private static void GreenMessage(string title, string text) =>
            ColorLog("4CA57D", "INFO", title, text);
        private static void OrangeMessage(string title, string text) =>
            ColorLogWarning("D16A31", "WARN", title, text);
        private static void RedMessage(string title, string text) =>
            ColorLogError("CE342A", "ERR", title, text);
        private static void LogMessage(string title, string text)
        {
            if(text != null)
                Debug.Log($"{title}: {text}");
            else
                Debug.Log($"{title}");
        }
            


        public static void Info(string title = "", string text = null) => GreenMessage(title, text);
        public static void Warn(string title = "", string text = null) => OrangeMessage(title, text);

        public static void Error(string title = "", string text = null) => RedMessage(title, text);

        public static void Log(string title = "", object text = null) =>
            LogMessage(title, text?.ToString());
        
        public static void Log(object value) =>
            LogMessage(value.ToString(), null);

        public static void ModuleLog(string log, ModuleName name) =>
                Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");
        

        public static void SystemLog(string text) =>
            Debug.Log("<color=#04bc04> GP: </color> " + $"{text}");
    }

}
