using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class Logger : MonoBehaviour
    {
        private static void ColorMessage(string color, string type, string title, string text) =>
            Debug.Log($"<color=#{color}> {type}: </color> {title}: {text}");


        private static void GreenMessage(string title, string text) =>
            ColorMessage("4CA57D", "INFO", title, text);
        private static void OrangeMessage(string title, string text) =>
            ColorMessage("D16A31", "WARN", title, text);
        private static void RedMessage(string title, string text) =>
            ColorMessage("CE342A", "ERR", title, text);
        private static void LogMessage(string title, string text) =>
            Debug.Log($"{title}: {text}");


        public static void Info(string title = "", string text = null) => GreenMessage(title, text);
        public static void Warn(string title = "", string text = null) => OrangeMessage(title, text);

        public static void Error(string title = "", string text = null) => RedMessage(title, text);

        public static void Log(string title = "", string text = null) => LogMessage(title, text);

        public static void ModuleLog(string log, ModuleName name) =>
                Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");
        

        public static void SystemLog(string text) =>
            Debug.Log("<color=#04bc04> GP: </color> " + $"{text}");
    }

}
