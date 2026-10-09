using UnityEngine;
using GamePush.ConsoleController;
using GamePush.Data;

namespace GamePush
{
    public class GP_Logger : MonoBehaviour
    {
        public static bool FullLogs
        {
            get
            {
#if UNITY_EDITOR
                return GP_Settings.instance != null && GP_Settings.instance.fullLogs;
#else
                return ProjectData.FULL_LOGS;
#endif
            }
        }

        public static void Info(string title = "", string text = null)
        {
            if (!FullLogs)
                return;
            Write("info", title, text);
        }

        public static void Warn(string title = "", string text = null) =>
            Write("warn", title, text);

        public static void Error(string title = "", string text = null) =>
            Write("error", title, text);

        public static void Log(string title = "", string text = null)
        {
            if (!FullLogs)
                return;
            Write("info", title, text);
        }

        public static void ModuleLog(string log, ModuleName name)
        {
#if UNITY_EDITOR
            if (GP_ConsoleController.Instance.IsModuleLogs(name))
                Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");
#else
            if (GP_Settings.instance.viewLogs)
                Debug.Log("<color=#04bc04> GP: </color> " + $"{name}: {log}");
#endif
        }

        public static void SystemLog(string text)
        {
            Debug.Log("[GP] System: " + text);
#if UNITY_EDITOR || !UNITY_WEBGL
            if (FullLogs)
                GP_LoggerToasts.Push("info", "System", text);
#endif
        }

        static void Write(string kind, string title, string text)
        {
            var line = string.IsNullOrEmpty(text) ? "[GP] " + title : "[GP] " + title + ": " + text;
            if (kind == "error")
                Debug.LogError(line);
            else if (kind == "warn")
                Debug.LogWarning(line);
            else
                Debug.Log(line);
#if UNITY_EDITOR || !UNITY_WEBGL
            GP_LoggerToasts.Push(kind, title, text);
#endif
        }
    }
}
