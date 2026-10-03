using System;
using System.Threading;
using GamePush;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    [InitializeOnLoad]
    static class GP_Play2WebHost
    {
        static readonly object _flushLock = new object();
        static GP_Play2WebServer _server;
        static Thread _flushThread;
        static volatile bool _flushRunning;
        static bool _subscribed;
        static bool _session;

        static GP_Play2WebHost()
        {
            EditorApplication.playModeStateChanged += OnPlayMode;
            EditorApplication.update += OnUpdate;
            AssemblyReloadEvents.beforeAssemblyReload += OnDomainUnload;
            AssemblyReloadEvents.afterAssemblyReload += OnDomainReloaded;
            EditorApplication.quitting += OnQuit;
            // During a domain reload Unity briefly reports isPlaying=false even if Play
            // Test is still coming back. Never clear Requested here — EnteredEditMode does.
            if (GP_Play2Web.Requested)
                EditorApplication.delayCall += EnsureRunningIfNeeded;
        }

        static void OnQuit()
        {
            Stop(clearRequested: true);
            GP_Play2WebOverlay.Shutdown();
        }

        static void OnDomainUnload()
        {
            Stop(clearRequested: false);
            GP_Play2WebOverlay.Shutdown();
        }

        static void OnDomainReloaded()
        {
            if (!GP_Play2Web.Requested)
                return;
            EditorApplication.delayCall += EnsureRunningIfNeeded;
        }

        static void EnsureRunningIfNeeded()
        {
            if (!GP_Play2Web.Requested)
                return;
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            EnsureRunning();
        }

        static void OnPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (GP_Play2Web.Requested)
                    StartServer();
                else
                    Stop(clearRequested: true);
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (GP_Play2Web.Requested)
                    EditorApplication.delayCall += EnsureRunning;
            }
            else if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    Stop(clearRequested: true);
            }
        }

        static void EnsureRunning()
        {
            if (!GP_Play2Web.Requested)
                return;
            StartServer();
            if (_server != null)
                GP_Play2WebOverlay.Launch(_server.Origin);
            SubscribeRuntime();
        }

        static void StartServer()
        {
            if (_session && _server != null)
                return;

            var root = GP_Play2WebTemplateComposer.Compose(!GP_Play2WebOverlay.IsRunning);
            _server?.Stop();
            _server = new GP_Play2WebServer(root);

            var port = EditorPrefs.GetInt(GP_Play2Web.PortPref, GP_Play2Web.DefaultPort);
            if (!_server.Start(port))
            {
                Debug.LogError("[Play2Web] Failed to start local server");
                return;
            }
            _session = true;
            EditorPrefs.SetInt(GP_Play2Web.PortPref, _server.Port);
            _server.Log += GP_Play2WebWindow.PushLog;
            _server.ClientMessage += OnClientMessage;
            StartFlushThread();
            GP_Play2WebWindow.PushLog("Origin " + _server.Origin + " — add it as a test site in the GamePush panel");
            if (EditorApplication.isPlaying)
                GP_Play2WebWindow.PushLog("Play2Web host running");
        }

        static void Stop(bool clearRequested)
        {
            UnsubscribeRuntime();
            StopFlushThread();
            GP_Play2WebOverlay.Stop();
            lock (_flushLock)
            {
                if (_server != null)
                {
                    _server.Log -= GP_Play2WebWindow.PushLog;
                    _server.ClientMessage -= OnClientMessage;
                    _server.Stop();
                    _server = null;
                }
            }
            if (clearRequested)
                GP_Play2Web.Requested = false;
            _session = false;
        }

        static void SubscribeRuntime()
        {
            if (_subscribed)
                return;
            GP_Play2Web.OverlayVisibilityChanged += OnOverlay;
            GP_Play2Web.Log += GP_Play2WebWindow.PushLog;
            GP_Play2Web.FlushToBrowser += FlushBrowser;
            _subscribed = true;
        }

        static void UnsubscribeRuntime()
        {
            if (!_subscribed)
                return;
            GP_Play2Web.OverlayVisibilityChanged -= OnOverlay;
            GP_Play2Web.Log -= GP_Play2WebWindow.PushLog;
            GP_Play2Web.FlushToBrowser -= FlushBrowser;
            _subscribed = false;
        }

        static void StartFlushThread()
        {
            if (_flushRunning)
                return;
            _flushRunning = true;
            _flushThread = new Thread(FlushLoop)
            {
                IsBackground = true,
                Name = "GP_Play2WebFlush"
            };
            _flushThread.Start();
        }

        static void StopFlushThread()
        {
            _flushRunning = false;
            var thread = _flushThread;
            _flushThread = null;
            if (thread != null && thread.IsAlive && thread != Thread.CurrentThread)
                thread.Join(500);
        }

        static void FlushLoop()
        {
            while (_flushRunning)
            {
                FlushBrowser();
                Thread.Sleep(16);
            }
        }

        static void FlushBrowser()
        {
            lock (_flushLock)
            {
                if (_server == null)
                    return;
                while (GP_Play2WebBus.TryTakeToBrowser(out var json))
                    _server.Broadcast(json);
            }
        }

        static void OnOverlay(bool visible)
        {
            GP_Play2WebOverlay.SetVisible(visible);
        }

        static void OnClientMessage(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;
            GP_Play2WebBus.SendToUnity(json);

            // log / ready already reach the window via GP_Play2Web.Log.
            // snapshot / overlay are cache and UI sync, not something to dump as JSON.
            var type = JsonString(json, "type");
            if (type == "log" || type == "ready" || type == "snapshot" || type == "overlay")
                return;
            if (type == "event")
            {
                var method = JsonString(json, "method") ?? "";
                if (string.IsNullOrEmpty(method) || IsQuietEvent(method))
                    return;
                var line = "event " + method;
                EditorApplication.delayCall += () => GP_Play2WebWindow.PushLog(line);
                return;
            }

            var snippet = json.Length > 180 ? json.Substring(0, 180) + "…" : json;
            EditorApplication.delayCall += () => GP_Play2WebWindow.PushLog(snippet);
        }

        static bool IsQuietEvent(string method)
        {
            return method.IndexOf("Tick", StringComparison.OrdinalIgnoreCase) >= 0
                   || method == "CallPlayerChange";
        }

        static string JsonString(string json, string key)
        {
            var needle = "\"" + key + "\":\"";
            var start = json.IndexOf(needle, StringComparison.Ordinal);
            if (start < 0)
                return "";
            start += needle.Length;
            var end = json.IndexOf('"', start);
            return end < 0 ? "" : json.Substring(start, end - start);
        }

        static void OnUpdate()
        {
            FlushBrowser();

            if (!EditorApplication.isPlaying || !GP_Play2Web.Requested)
                return;
            // Overlay focus throttles the player loop, so Pump() cannot live only on
            // MonoBehaviour.Update — channel events would sit in the bus until timeout.
            GP_Play2Web.Pump();
            GP_Play2WebOverlay.Tick();
        }
    }
}
