#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
#if UNITY_EDITOR_WIN
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
#endif
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    // The overlay lives in a separate browser-host process (WebView2 on Windows, WKWebView
    // on macOS). Hosting the engine inside the editor needs its own pump and turns any
    // browser fault into an editor crash, so Unity only compiles the host, starts it and
    // streams the Game view rect over stdin.
    static class GP_Play2WebOverlay
    {
        // The host window composites the page with per-pixel alpha, so everything the page does
        // not paint stays see-through and the Game view shows through it.
        public const string PageBackgroundCss = "transparent";

        const string PidPref = "GamePush.Play2Web.HostPid";
        const float GameViewToolbarPoints = 21f;
#if UNITY_EDITOR_WIN
        const uint GaRoot = 2;
#endif

        static Process _process;
        static bool _cover;
        static bool _ready;
        static bool _hidden;
        static RectInt _lastRect;
        static IntPtr _lastOwner;
#if UNITY_EDITOR_WIN
        static readonly Dictionary<MethodInfo, Func<object, IntPtr>> RefIntPtrGetters =
            new Dictionary<MethodInfo, Func<object, IntPtr>>();
#endif

        public static string State =>
            $"host={(IsRunning ? "up" : "down")} ready={_ready} cover={_cover} rect={_lastRect.width}x{_lastRect.height}";

        public static bool IsRunning
        {
            get
            {
                try { return _process != null && !_process.HasExited; }
                catch { return false; }
            }
        }

        public static void Launch(string origin)
        {
            if (IsRunning)
            {
                Send("nav " + origin);
                Tick();
                return;
            }

            KillOrphanHost();
            var exe = GP_Play2WebHostBuilder.EnsureBuilt();
            if (string.IsNullOrEmpty(exe))
                return;

            var userData = Path.Combine(
                Directory.GetParent(Application.dataPath)?.FullName ?? "",
                "Temp", "GamePushPlay2Web",
#if UNITY_EDITOR_WIN
                "webview2-profile");
#else
                "wkwebview-profile");
#endif
            Directory.CreateDirectory(userData);

            var info = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = $"--url \"{origin}\" --user-data \"{userData}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _cover = false;
            _ready = false;
            _hidden = false;
            _lastRect = default;
            _lastOwner = IntPtr.Zero;

            try
            {
                _process = Process.Start(info);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[Play2Web] Could not start overlay host: " + ex.Message);
                return;
            }

            if (_process == null)
                return;

            _process.OutputDataReceived += OnHostOutput;
            _process.ErrorDataReceived += OnHostOutput;
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            EditorPrefs.SetInt(PidPref, _process.Id);
            Tick();
        }

        public static void Stop()
        {
            _ready = false;
            _cover = false;
            _lastRect = default;
            _lastOwner = IntPtr.Zero;
            var process = _process;
            _process = null;
            EditorPrefs.DeleteKey(PidPref);
            if (process == null)
                return;

            try
            {
                if (!process.HasExited)
                {
                    process.StandardInput.WriteLine("quit");
                    process.StandardInput.Flush();
                    if (!process.WaitForExit(700))
                        process.Kill();
                }
            }
            catch
            {
                try { process.Kill(); }
                catch { /* already gone */ }
            }
        }

        public static void Shutdown() => Stop();

        public static void SetVisible(bool visible)
        {
            _cover = visible;
            Send("cover " + (visible ? "1" : "0"));
            Tick();
        }

        public static void FollowGameView() => Tick();

        public static void Tick()
        {
            if (!IsRunning)
                return;

            var gameView = FindGameView();
            var owner = OverlayOwner(gameView);
            if (owner != IntPtr.Zero && owner != _lastOwner)
            {
                _lastOwner = owner;
                Send("owner " + owner.ToInt64().ToString(CultureInfo.InvariantCulture));
            }

            var rect = GameViewScreenRect(gameView);
            if (rect.width < 2 || rect.height < 2
                || !IsGameViewTabVisible(gameView)
#if UNITY_EDITOR_WIN
                || !EditorInForeground(owner)
#endif
                )
            {
                if (!_hidden)
                {
                    _hidden = true;
                    Send("hide");
                }
                return;
            }

            if (!_hidden && rect.Equals(_lastRect))
                return;
            _hidden = false;
            _lastRect = rect;
            Send($"rect {rect.x} {rect.y} {rect.width} {rect.height}");
        }

        static IntPtr OverlayOwner(EditorWindow gameView)
        {
#if UNITY_EDITOR_WIN
            return GameViewRootHwnd(gameView);
#else
            return new IntPtr(Process.GetCurrentProcess().Id);
#endif
        }

        static void Send(string command)
        {
            if (!IsRunning)
                return;
            try
            {
                _process.StandardInput.WriteLine(command);
                _process.StandardInput.Flush();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("[Play2Web] Overlay host pipe closed: " + ex.Message);
                _process = null;
            }
        }

        static void OnHostOutput(object sender, DataReceivedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Data))
                return;
            var line = args.Data;
            EditorApplication.delayCall += () =>
            {
                if (line == "ready")
                    _ready = true;
                GP_Play2WebWindow.PushLog("host: " + line);
                if (line.StartsWith("err ", StringComparison.Ordinal))
                    UnityEngine.Debug.LogWarning("[Play2Web] " + line);
            };
        }

        static void KillOrphanHost()
        {
            var pid = EditorPrefs.GetInt(PidPref, 0);
            EditorPrefs.DeleteKey(PidPref);
            if (pid <= 0)
                return;
            try
            {
                var process = Process.GetProcessById(pid);
                if (!process.HasExited && process.ProcessName.IndexOf("gp-play2web", StringComparison.OrdinalIgnoreCase) >= 0)
                    process.Kill();
            }
            catch { /* already gone */ }
        }

        // Cover the pixels the player actually sees. targetInContent is zoom-content space
        // (origin at the centre, Scale ignored) so on a Retina Mac the default 2x Game view
        // scale leaves a half-size overlay sitting in the middle. targetInParent / targetInView
        // already include that scale, the toolbar, and letterboxing.
        static RectInt GameViewScreenRect(EditorWindow gameView)
        {
            if (gameView == null)
                return default;

            var area = GameViewVisibleRect(gameView);
            if (area.width < 2f || area.height < 2f)
                return default;

            // Win32 SetWindowPos is pixels; Cocoa NSWindow.setFrame is points (Unity's native unit).
#if UNITY_EDITOR_WIN
            var scale = EditorGUIUtility.pixelsPerPoint <= 0f ? 1f : EditorGUIUtility.pixelsPerPoint;
#else
            var scale = 1f;
#endif
            return new RectInt(
                Mathf.RoundToInt(area.x * scale),
                Mathf.RoundToInt(area.y * scale),
                Mathf.Max(2, Mathf.RoundToInt(area.width * scale)),
                Mathf.Max(2, Mathf.RoundToInt(area.height * scale)));
        }

        static Rect GameViewVisibleRect(EditorWindow gameView)
        {
            var parentScreen = ParentScreenPosition(gameView);
            var content = GameViewContentRect(gameView, parentScreen);
            var placed = GameViewTargetRect(gameView, parentScreen, content);
            return placed.HasValue ? Intersect(placed.Value, content) : content;
        }

        static Rect GameViewContentRect(EditorWindow gameView, Rect? parentScreen)
        {
            if (parentScreen.HasValue)
            {
                var viewInParent = ReadRect(gameView, "viewInParent");
                if (viewInParent.HasValue && viewInParent.Value.width > 1f && viewInParent.Value.height > 1f)
                    return new Rect(
                        parentScreen.Value.x + viewInParent.Value.x,
                        parentScreen.Value.y + viewInParent.Value.y,
                        viewInParent.Value.width,
                        viewInParent.Value.height);

                var tab = TabStripHeight(gameView, parentScreen);
                return new Rect(
                    parentScreen.Value.x,
                    parentScreen.Value.y + tab + GameViewToolbarPoints,
                    parentScreen.Value.width,
                    Mathf.Max(1f, parentScreen.Value.height - tab - GameViewToolbarPoints));
            }

            // EditorWindow.position starts at the top of the dock area, tab strip included,
            // while its height already excludes that strip.
            var view = gameView.position;
            var strip = TabStripHeight(gameView, null);
            return new Rect(
                view.x,
                view.y + strip + GameViewToolbarPoints,
                view.width,
                Mathf.Max(1f, view.height - GameViewToolbarPoints));
        }

        static Rect? GameViewTargetRect(EditorWindow gameView, Rect? parentScreen, Rect content)
        {
            if (parentScreen.HasValue)
            {
                var targetInParent = ReadRect(gameView, "targetInParent");
                if (targetInParent.HasValue && targetInParent.Value.width > 1f && targetInParent.Value.height > 1f)
                    return new Rect(
                        parentScreen.Value.x + targetInParent.Value.x,
                        parentScreen.Value.y + targetInParent.Value.y,
                        targetInParent.Value.width,
                        targetInParent.Value.height);
            }

            var targetInView = ReadRect(gameView, "targetInView");
            if (targetInView.HasValue && targetInView.Value.width > 1f && targetInView.Value.height > 1f)
                return new Rect(
                    content.x + targetInView.Value.x,
                    content.y + targetInView.Value.y,
                    targetInView.Value.width,
                    targetInView.Value.height);

            return null;
        }

        static Rect Intersect(Rect a, Rect b)
        {
            var xMin = Mathf.Max(a.xMin, b.xMin);
            var yMin = Mathf.Max(a.yMin, b.yMin);
            var xMax = Mathf.Min(a.xMax, b.xMax);
            var yMax = Mathf.Min(a.yMax, b.yMax);
            if (xMax - xMin < 2f || yMax - yMin < 2f)
                return b;
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        static float TabStripHeight(EditorWindow gameView, Rect? parentScreen)
        {
            var screen = parentScreen ?? ParentScreenPosition(gameView);
            if (!screen.HasValue)
                return 0f;

            var strip = screen.Value.height - gameView.position.height;
            return strip > 0f && strip < 60f ? strip : 0f;
        }

        static Rect? ParentScreenPosition(EditorWindow window)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var parent = typeof(EditorWindow).GetField("m_Parent", flags)?.GetValue(window);
            return ReadRect(parent, "screenPosition");
        }

        static Rect? ReadRect(object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return null;

            const BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            for (var type = obj.GetType(); type != null && type != typeof(object); type = type.BaseType)
            {
                var property = type.GetProperty(name, flags);
                if (property == null)
                    continue;
                try
                {
                    if (property.GetValue(obj) is Rect rect)
                        return rect;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        static EditorWindow FindGameView()
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            if (type == null)
                return null;
            var windows = Resources.FindObjectsOfTypeAll(type);
            return windows != null && windows.Length > 0 ? windows[0] as EditorWindow : null;
        }

        static bool IsGameViewTabVisible(EditorWindow gameView)
        {
            if (gameView == null)
                return false;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var parent = typeof(EditorWindow).GetField("m_Parent", flags)?.GetValue(gameView);
            if (parent == null)
                return false;

            var actual = parent.GetType().GetProperty("actualView", flags)?.GetValue(parent) as EditorWindow;
            if (actual != gameView)
                return false;

            var visibleProp = parent.GetType().GetProperty("visible", flags);
            if (visibleProp != null && visibleProp.PropertyType == typeof(bool)
                && visibleProp.GetValue(parent) is bool visible && !visible)
                return false;

            return true;
        }

#if UNITY_EDITOR_WIN
        // Hidden while Unity is minimized or in the background, restored as soon as it comes
        // back. Focus on the overlay itself counts as the editor being active, otherwise
        // clicking an ad would hide the ad. Iconic is checked on the Game view container, not
        // Process.MainWindowHandle — Unity's "main" window is often not the one that minimized.
        static bool EditorInForeground(IntPtr gameViewHwnd)
        {
            if (gameViewHwnd != IntPtr.Zero && IsIconic(gameViewHwnd))
                return false;

            var foreground = GetForegroundWindow();
            if (foreground == IntPtr.Zero)
                return false;

            GetWindowThreadProcessId(foreground, out var pid);
            if (pid == (uint)Process.GetCurrentProcess().Id)
                return true;
            return _process != null && pid == (uint)_process.Id;
        }

        static IntPtr GameViewRootHwnd(EditorWindow gameView)
        {
            if (gameView == null)
                return IntPtr.Zero;

            try
            {
                var parent = ReadInstance(gameView, "m_Parent");
                var hwnd = RootWindow(ReadHwnd(parent));
                if (hwnd != IntPtr.Zero)
                    return hwnd;

                var container = ReadInstance(parent, "window") ?? ReadInstance(parent, "m_Window");
                hwnd = RootWindow(ReadHwnd(container));
                if (hwnd != IntPtr.Zero)
                    return hwnd;
            }
            catch (Exception)
            {
                // Unity 6.6+ nativeHandle is `ref IntPtr`; PropertyInfo.GetValue throws
                // NotSupportedException. Field reads and the point fallback still work.
            }

            // nativeHandle is sometimes a C++ object, not an HWND. The window under the Game
            // view belongs to Unity; skip hits that are not our process (the overlay itself).
            var view = gameView.position;
            var scale = EditorGUIUtility.pixelsPerPoint <= 0f ? 1f : EditorGUIUtility.pixelsPerPoint;
            var point = new POINT
            {
                X = Mathf.RoundToInt((view.x + view.width * 0.5f) * scale),
                Y = Mathf.RoundToInt((view.y + view.height * 0.5f) * scale)
            };
            return RootWindow(WindowFromPoint(point));
        }

        // Older Unity: nativeHandle / winHandle return IntPtr and GetValue works.
        // Unity 6.6: those getters are `ref IntPtr` over MonoReloadableIntPtr fields.
        static IntPtr ReadHwnd(object obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            foreach (var name in new[] { "m_WindowPtr", "winHandle", "windowHandle", "nativeHandle", "m_ViewPtr" })
            {
                var hwnd = AsHwnd(ReadInstance(obj, name));
                if (hwnd != IntPtr.Zero)
                    return hwnd;
            }
            return IntPtr.Zero;
        }

        static object ReadInstance(object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return null;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            for (var type = obj.GetType(); type != null && type != typeof(object); type = type.BaseType)
            {
                var field = type.GetField(name, flags);
                if (field != null)
                    return field.GetValue(obj);

                var property = type.GetProperty(name, flags);
                if (property != null)
                    return ReadProperty(property, obj);
            }
            return null;
        }

        static object ReadProperty(PropertyInfo property, object obj)
        {
            var getter = property.GetGetMethod(true);
            if (getter == null)
                return null;
            if (getter.ReturnType.IsByRef)
                return ReadByRefIntPtr(getter, obj);

            try
            {
                return property.GetValue(obj);
            }
            catch (NotSupportedException)
            {
                return ReadByRefIntPtr(getter, obj);
            }
        }

        static object ReadByRefIntPtr(MethodInfo getter, object instance)
        {
            if (getter == null || instance == null)
                return null;
            var element = getter.ReturnType.IsByRef ? getter.ReturnType.GetElementType() : getter.ReturnType;
            if (element != typeof(IntPtr))
                return null;

            if (!RefIntPtrGetters.TryGetValue(getter, out var read))
            {
                read = BuildRefIntPtrGetter(getter);
                RefIntPtrGetters[getter] = read;
            }
            return read != null ? (object)read(instance) : null;
        }

        static Func<object, IntPtr> BuildRefIntPtrGetter(MethodInfo getter)
        {
            try
            {
                var method = new DynamicMethod(
                    "GP_ReadRefIntPtr_" + getter.DeclaringType.Name + "_" + getter.Name,
                    typeof(IntPtr),
                    new[] { typeof(object) },
                    getter.DeclaringType.Module,
                    skipVisibility: true);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, getter.DeclaringType);
                il.Emit(getter.IsVirtual && !getter.IsFinal ? OpCodes.Callvirt : OpCodes.Call, getter);
                il.Emit(OpCodes.Ldobj, typeof(IntPtr));
                il.Emit(OpCodes.Ret);
                return (Func<object, IntPtr>)method.CreateDelegate(typeof(Func<object, IntPtr>));
            }
            catch
            {
                return null;
            }
        }

        static IntPtr AsHwnd(object value)
        {
            if (value is IntPtr ptr)
                return ptr;
            if (value == null)
                return IntPtr.Zero;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var field = value.GetType().GetField("m_IntPtr", flags);
            return field != null && field.GetValue(value) is IntPtr inner ? inner : IntPtr.Zero;
        }

        static IntPtr RootWindow(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, out var pid);
            if (pid != (uint)Process.GetCurrentProcess().Id)
                return IntPtr.Zero;
            var root = GetAncestor(hwnd, GaRoot);
            return root != IntPtr.Zero ? root : hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll")]
        static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT point);
#endif
    }
}
#else
namespace GamePushEditor.Play2Web
{
    static class GP_Play2WebOverlay
    {
        public const string PageBackgroundCss = "transparent";
        public static string State => "unsupported platform";
        public static bool IsRunning => false;
        public static void Launch(string origin) { }
        public static void Stop() { }
        public static void Shutdown() { }
        public static void SetVisible(bool visible) { }
        public static void Tick() { }
        public static void FollowGameView() { }
    }
}
#endif
