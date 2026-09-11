using System.Collections.Concurrent;
using System.Collections.Generic;
using GamePush;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    public sealed class GP_Play2WebWindow : EditorWindow
    {
        static readonly List<string> Logs = new List<string>();
        static readonly ConcurrentQueue<string> PendingLogs = new ConcurrentQueue<string>();
        Vector2 _scroll;

        [InitializeOnLoadMethod]
        static void HookLogDrain()
        {
            EditorApplication.update -= DrainPendingLogs;
            EditorApplication.update += DrainPendingLogs;
        }

        public static void Open()
        {
            var window = GetWindow<GP_Play2WebWindow>();
            window.titleContent = new GUIContent("Play2Web");
            window.minSize = new Vector2(280, 220);
            window.Show();
        }

        const string LogsStateKey = "GamePush.Play2Web.WindowLogs";

        public static void ClearLog()
        {
            while (PendingLogs.TryDequeue(out _)) { }
            Logs.Clear();
            SessionState.EraseString(LogsStateKey);
        }

        public static void PushLog(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            PendingLogs.Enqueue(message);
        }

        static void DrainPendingLogs()
        {
            if (PendingLogs.IsEmpty)
                return;
            RestoreLogsIfEmpty();
            while (PendingLogs.TryDequeue(out var message))
            {
                if (string.IsNullOrEmpty(message))
                    continue;
                if (Logs.Count > 0 && Logs[Logs.Count - 1] == message)
                    continue;
                Logs.Add(message);
            }
            if (Logs.Count > 200)
                Logs.RemoveRange(0, Logs.Count - 200);
            PersistLogs();
        }

        static void RestoreLogsIfEmpty()
        {
            if (Logs.Count > 0)
                return;
            var saved = SessionState.GetString(LogsStateKey, "");
            if (string.IsNullOrEmpty(saved))
                return;
            var lines = saved.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrEmpty(lines[i]))
                    Logs.Add(lines[i]);
            }
        }

        static void PersistLogs()
        {
            var from = Mathf.Max(0, Logs.Count - 80);
            SessionState.SetString(LogsStateKey, string.Join("\n", Logs.GetRange(from, Logs.Count - from)));
        }

        void OnGUI()
        {
            DrainPendingLogs();
            EditorGUILayout.LabelField("Play2Web", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Beta feature, so expect rough edges.\n\nUse Play Test below to overlay the live GamePush isDev UI on Game view (ads, purchases, popups). The regular Play button does not start this mode.\nAdd http://127.0.0.1:<port>/ as a test origin in the GamePush panel.",
                MessageType.Info);

            EditorGUILayout.LabelField("Playing", EditorApplication.isPlaying ? "Yes" : "No");
            EditorGUILayout.LabelField("Ready", GP_Play2Web.IsReady ? "Yes" : "No");
            EditorGUILayout.LabelField("Overlay", GP_Play2WebOverlay.State);
            EditorGUILayout.LabelField("Port", EditorPrefs.GetInt(GP_Play2Web.PortPref, GP_Play2Web.DefaultPort).ToString());
            EditorGUILayout.LabelField("Template", PlayerSettings.WebGL.template);
            EditorGUILayout.LabelField("Init scene", InitSceneStatus());

            DrawPlayTestButtons();

            if (GUILayout.Button("Open GamePush settings"))
                EditorApplication.ExecuteMenuItem("Tools/GamePush");

            GUILayout.Space(8);
            EditorGUILayout.LabelField("Log", EditorStyles.boldLabel);
            RestoreLogsIfEmpty();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            for (var i = Mathf.Max(0, Logs.Count - 80); i < Logs.Count; i++)
                EditorGUILayout.SelectableLabel(Logs[i], EditorStyles.miniLabel, GUILayout.Height(16));
            EditorGUILayout.EndScrollView();
        }

        void DrawPlayTestButtons()
        {
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                var compiling = EditorApplication.isCompiling;
                using (new EditorGUI.DisabledScope(compiling))
                {
                    var playLabel = EditorApplication.isPlaying ? "Restart Play Test" : "Play Test";
                    if (GUILayout.Button(playLabel, GUILayout.Height(28)))
                        GP_Play2WebPlayTest.Start();
                }

                using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
                {
                    if (GUILayout.Button("Stop", GUILayout.Height(28), GUILayout.Width(72)))
                        GP_Play2WebPlayTest.Stop();
                }
            }

            EditorGUILayout.HelpBox(
                "Play Test always boots AwaitInit (build index 0), even if another scene is open. The editor scene is left as-is.",
                MessageType.None);
        }

        static string InitSceneStatus()
        {
            if (!GP_InitSceneUtility.Exists)
                return "missing";
            var index = GP_InitSceneUtility.BuildIndex;
            if (index == 0)
                return "AwaitInit @ 0";
            if (index > 0)
                return "AwaitInit @ " + index + " (will move to 0)";
            return "not in Build Settings (will add as 0)";
        }

        void OnInspectorUpdate() => Repaint();
    }
}
