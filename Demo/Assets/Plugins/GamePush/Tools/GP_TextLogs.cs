using System.IO;
using UnityEngine;

namespace GamePush.Tools
{
    public class GP_TextLogs : MonoBehaviour
    {
        private string logPath;

        void Start()
        {
            logPath = Path.Combine(Application.persistentDataPath, "gp_logs.txt");
            using (FileStream fs = File.Create(logPath)) { }
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    writer.WriteLine($"{type}: {logString}\n{stackTrace}");
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to write log to {logPath}: {e.Message}");
            }
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }


}
