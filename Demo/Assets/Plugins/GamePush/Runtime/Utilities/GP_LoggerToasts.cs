using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using GamePush.Data;
using GamePush.Native;
using UnityEngine;

namespace GamePush
{
    [DefaultExecutionOrder(10000)]
    public sealed class GP_LoggerToasts : MonoBehaviour
    {
        const int MaxLines = 32;
        const float Pad = 24f;
        const float Gap = 4f;
        const float Width = 460f;
        const float CopyWidth = 108f;
        static readonly Color HeaderColor = new Color(0.12f, 0.12f, 0.14f, 0.92f);
        static readonly Color InfoColor = new Color(0.055f, 0.659f, 0.478f, 0.94f);
        static readonly Color WarnColor = new Color(0.878f, 0.388f, 0.102f, 0.94f);
        static readonly Color ErrorColor = new Color(0.878f, 0.102f, 0.102f, 0.94f);

        static readonly ConcurrentQueue<Pending> Incoming = new ConcurrentQueue<Pending>();
        static readonly List<Item> Shown = new List<Item>();

        Texture2D _pixel;
        GUIStyle _titleStyle;
        GUIStyle _bodyStyle;
        GUIStyle _copyStyle;
        bool _expanded;
        bool _dragging;
        Vector2 _scroll;
        float _copiedUntil;

        public static void Push(string kind, string title, string text)
        {
#if UNITY_EDITOR || (UNITY_WEBGL && !UNITY_EDITOR)
            return;
#else
            if (!ProjectData.NATIVE_DEBUG_CONSOLE)
                return;
            Incoming.Enqueue(new Pending
            {
                kind = kind ?? "info",
                title = title ?? "",
                text = text ?? ""
            });
#endif
        }

        static bool OverlayEnabled
        {
            get
            {
#if UNITY_EDITOR || (UNITY_WEBGL && !UNITY_EDITOR)
                return false;
#else
                if (!ProjectData.NATIVE_DEBUG_CONSOLE)
                    return false;
                if (Application.isBatchMode)
                    return false;
                if (GamePushHost.UseNativeCore)
                    return true;
                return NativeCore.IsDev;
#endif
            }
        }

        void OnEnable()
        {
            _pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _pixel.SetPixel(0, 0, Color.white);
            _pixel.Apply();
        }

        void OnDisable()
        {
            if (_pixel != null)
                Destroy(_pixel);
            _pixel = null;
        }

        void OnGUI()
        {
            Drain();
            if (!OverlayEnabled)
                return;
            if (_pixel == null)
                return;

            EnsureStyles();
            GUI.depth = -2000;

            var gui = GUIUtility.ScreenToGUIRect(new Rect(0, 0, Screen.width, Screen.height));
            var guiW = gui.width > 1f ? gui.width : Screen.width;
            var guiH = gui.height > 1f ? gui.height : Screen.height;
            var x = gui.x + guiW - Pad - Width;
            var y = gui.y + guiH - Pad;

            var status = NativeCore.Ready
                ? "GP Native  id=" + NativePlayer.Id + "  " + NativeCore.PlatformType + "/" + NativeCore.PlatformTag +
                  "  isDev=" + NativeCore.IsDev
                : "GP Native  connecting…";
            var header = (_expanded ? "▾  " : "▸  ") + status;

            var headerHeight = Measure(header, "");
            var headerRect = new Rect(x, y - headerHeight, Width, headerHeight);

            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && headerRect.Contains(e.mousePosition))
            {
                _expanded = !_expanded;
                _dragging = false;
                e.Use();
            }

            if (e.type == EventType.Repaint)
                DrawBlock(header, "", HeaderColor, x, y);

            if (!_expanded)
                return;

            var viewHeight = Mathf.Min(guiH * 0.33f, headerRect.y - Pad - Gap);
            if (viewHeight < 48f)
                return;

            var viewRect = new Rect(x, headerRect.y - Gap - viewHeight, Width, viewHeight);
            var copyRect = new Rect(x - Gap - CopyWidth, headerRect.y, CopyWidth, headerHeight);
            if (copyRect.x < gui.x + Pad)
                copyRect.x = gui.x + Pad;

            if (e.type == EventType.Repaint)
                DrawFill(copyRect, HeaderColor);
            if (GUI.Button(copyRect, Time.unscaledTime < _copiedUntil ? "Copied" : "Copy", _copyStyle))
            {
                GUIUtility.systemCopyBuffer = BuildCopyText(status);
                _copiedUntil = Time.unscaledTime + 1.5f;
            }

            var contentWidth = Width - 16f;
            var contentHeight = 0f;
            for (var i = Shown.Count - 1; i >= 0; i--)
            {
                contentHeight += Measure(Shown[i].title, Shown[i].text, contentWidth);
                if (i > 0)
                    contentHeight += Gap;
            }

            HandleLogScroll(e, viewRect, copyRect, headerRect, contentHeight, viewHeight);

            var contentRect = new Rect(0f, 0f, contentWidth, Mathf.Max(contentHeight, viewHeight));
            _scroll = GUI.BeginScrollView(viewRect, _scroll, contentRect, false, true);
            var itemY = 0f;
            var paint = Event.current.type == EventType.Repaint;
            for (var i = Shown.Count - 1; i >= 0; i--)
            {
                var item = Shown[i];
                var height = Measure(item.title, item.text, contentWidth);
                if (paint)
                    DrawBlockTop(item.title, item.text, item.color, 0f, itemY, contentWidth);
                else
                    GUI.Box(new Rect(0f, itemY, contentWidth, height), GUIContent.none, GUIStyle.none);
                itemY += height + Gap;
            }

            GUI.EndScrollView();
        }

        void HandleLogScroll(Event e, Rect viewRect, Rect copyRect, Rect headerRect, float contentHeight,
            float viewHeight)
        {
            if (e == null)
                return;

            var overBar = e.mousePosition.x >= viewRect.xMax - 20f
                          && viewRect.Contains(e.mousePosition);
            var overLogs = viewRect.Contains(e.mousePosition)
                           && !overBar
                           && !copyRect.Contains(e.mousePosition)
                           && !headerRect.Contains(e.mousePosition);
            var maxScroll = Mathf.Max(0f, contentHeight - viewHeight);

            if (e.type == EventType.ScrollWheel && overLogs)
            {
                _scroll.y += e.delta.y * 16f;
                e.Use();
            }
            else if (e.type == EventType.MouseDown && e.button == 0 && overLogs)
            {
                _dragging = true;
                e.Use();
            }
            else if (_dragging && e.type == EventType.MouseDrag && e.button == 0)
            {
                _scroll.y -= e.delta.y;
                e.Use();
            }
            else if (_dragging && (e.type == EventType.MouseUp || e.type == EventType.Ignore))
            {
                _dragging = false;
                if (e.type == EventType.MouseUp)
                    e.Use();
            }

            _scroll.y = Mathf.Clamp(_scroll.y, 0f, maxScroll);
        }

        static string BuildCopyText(string status)
        {
            var sb = new StringBuilder();
            sb.AppendLine(status ?? "");
            foreach (var item in Shown)
            {
                if (!string.IsNullOrEmpty(item.sourceTitle))
                    sb.AppendLine(item.sourceTitle);
                if (!string.IsNullOrEmpty(item.sourceText))
                    sb.AppendLine(item.sourceText);
            }
            return sb.ToString();
        }

        float Measure(string title, string body, float width = 0f)
        {
            if (width <= 0f)
                width = Width;
            var titleHeight = _titleStyle.CalcHeight(new GUIContent(title ?? ""), width - 20f);
            var bodyHeight = string.IsNullOrEmpty(body)
                ? 0f
                : _bodyStyle.CalcHeight(new GUIContent(body), width - 20f);
            return 12f + titleHeight + (bodyHeight > 0f ? 4f + bodyHeight : 0f);
        }

        float DrawBlock(string title, string body, Color color, float x, float bottom)
        {
            title = title ?? "";
            body = body ?? "";
            var height = Measure(title, body);
            DrawBlockTop(title, body, color, x, bottom - height, Width);
            return height;
        }

        void DrawBlockTop(string title, string body, Color color, float x, float top, float width)
        {
            title = title ?? "";
            body = body ?? "";
            var titleHeight = _titleStyle.CalcHeight(new GUIContent(title), width - 20f);
            var bodyHeight = string.IsNullOrEmpty(body)
                ? 0f
                : _bodyStyle.CalcHeight(new GUIContent(body), width - 20f);
            var height = 12f + titleHeight + (bodyHeight > 0f ? 4f + bodyHeight : 0f);
            var rect = new Rect(x, top, width, height);
            DrawFill(rect, color);
            GUI.Label(new Rect(rect.x + 10f, rect.y + 6f, rect.width - 20f, titleHeight), title, _titleStyle);
            if (bodyHeight > 0f)
                GUI.Label(new Rect(rect.x + 10f, rect.y + 6f + titleHeight + 2f, rect.width - 20f, bodyHeight),
                    body, _bodyStyle);
        }

        void DrawFill(Rect rect, Color color)
        {
            var previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, _pixel);
            GUI.color = previous;
        }

        static string Wrap(string text, int maxChars)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxChars)
                return text;
            var sb = new System.Text.StringBuilder(text.Length + 8);
            for (var i = 0; i < text.Length; i += maxChars)
            {
                if (i > 0)
                    sb.Append('\n');
                var len = text.Length - i;
                if (len > maxChars)
                    len = maxChars;
                sb.Append(text, i, len);
            }
            return sb.ToString();
        }

        static void Drain()
        {
            while (Incoming.TryDequeue(out var pending))
            {
                var color = InfoColor;
                var prefix = "INFO: ";
                if (pending.kind == "warn")
                {
                    color = WarnColor;
                    prefix = "WARN: ";
                }
                else if (pending.kind == "error")
                {
                    color = ErrorColor;
                    prefix = "ERR: ";
                }

                var title = Wrap(prefix + pending.title, 52);
                var text = Wrap(pending.text, 52);
                var sourceTitle = prefix + (pending.title ?? "");
                var sourceText = pending.text ?? "";
                if (Shown.Count > 0)
                {
                    var last = Shown[Shown.Count - 1];
                    if (last.sourceTitle == sourceTitle && last.sourceText == sourceText)
                        continue;
                }
                Shown.Add(new Item
                {
                    title = title,
                    text = text,
                    sourceTitle = sourceTitle,
                    sourceText = sourceText,
                    color = color
                });
                while (Shown.Count > MaxLines)
                    Shown.RemoveAt(0);
            }
        }

        void EnsureStyles()
        {
            if (_titleStyle != null)
                return;
            var skin = GUI.skin != null ? GUI.skin.label : new GUIStyle();
            _titleStyle = new GUIStyle(skin)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                clipping = TextClipping.Clip
            };
            _titleStyle.normal.textColor = Color.white;
            _bodyStyle = new GUIStyle(skin)
            {
                fontSize = 12,
                wordWrap = true,
                clipping = TextClipping.Clip
            };
            _bodyStyle.normal.textColor = Color.white;
            var button = GUI.skin != null ? GUI.skin.button : new GUIStyle();
            _copyStyle = new GUIStyle(button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                alignment = TextAnchor.MiddleCenter
            };
            _copyStyle.normal.textColor = Color.white;
            _copyStyle.hover.textColor = Color.white;
            _copyStyle.active.textColor = Color.white;
        }

        struct Pending
        {
            public string kind;
            public string title;
            public string text;
        }

        struct Item
        {
            public string title;
            public string text;
            public string sourceTitle;
            public string sourceText;
            public Color color;
        }
    }
}
